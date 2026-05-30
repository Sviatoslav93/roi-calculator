using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RoiForm.Api.Behaviors;

internal sealed partial class GlobalExceptionHandler(
    IHostEnvironment env,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        LogUnhandledException(exception);

        var statusCode = GetStatusCode(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = env.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.",
            Instance = httpContext.Request.Path,
        };

        EnrichProblemDetails(problemDetails, httpContext, exception);

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private void EnrichProblemDetails(
        ProblemDetails problemDetails,
        HttpContext httpContext,
        Exception exception)
    {
        // Always useful in production
        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (!env.IsDevelopment())
        {
            return;
        }

        problemDetails.Extensions["requestId"] =
            httpContext.TraceIdentifier;

        problemDetails.Extensions["request"] = new
        {
            httpContext.Request.Method,
            httpContext.Request.Path,
            Query = httpContext.Request.Query.ToDictionary(
                q => q.Key,
                q => q.Value.ToString(),
                StringComparer.Ordinal),

            Headers = FilterHeaders(httpContext.Request.Headers),
        };

        problemDetails.Extensions["exception"] = new
        {
            Type = exception.GetType().FullName,
            exception.Message,
            exception.StackTrace,
        };

        if (exception.InnerException is not null)
        {
            problemDetails.Extensions["innerException"] = new
            {
                Type = exception.InnerException.GetType().FullName,
                exception.InnerException.Message,
                exception.InnerException.StackTrace,
            };
        }
    }

    private static Dictionary<string, string> FilterHeaders(
        IHeaderDictionary headers)
    {
        var excludedHeaders = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "Authorization",
            "Cookie",
            "Set-Cookie",
            "X-Api-Key",
        };

        return headers
            .Where(h => !excludedHeaders.Contains(h.Key))
            .ToDictionary(
                h => h.Key,
                h => h.Value.ToString(),
                StringComparer.OrdinalIgnoreCase);
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError,
        };

    private static string GetTitle(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad request",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Resource not found",
            StatusCodes.Status501NotImplemented => "Not implemented",
            _ => "Unhandled exception",
        };

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Unhandled exception occurred")]
    private partial void LogUnhandledException(Exception exception);
}
