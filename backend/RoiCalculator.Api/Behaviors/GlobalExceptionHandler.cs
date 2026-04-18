using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RoiCalculator.Api.Behaviors;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            InvalidOperationException ioe => (
                StatusCodes.Status422UnprocessableEntity,
                "Unprocessable Entity",
                ioe.Message),
            _ => (0, null!, null!)
        };

        if (status == 0) return false;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            },
            cancellationToken);

        return true;
    }
}
