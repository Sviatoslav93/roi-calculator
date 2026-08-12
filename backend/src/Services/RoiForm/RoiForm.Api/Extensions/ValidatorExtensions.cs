using FluentValidation;
using FunctionalPrimitives.Errors;
using FunctionalPrimitives.Errors.Extensions;
using FunctionalPrimitives.Monads.Results;

namespace RoiForm.Api.Extensions;

public static class ValidatorExtensions
{
    public static async Task<Result<T>> ValidateToResultAsync<T>(
        this IValidator<T> validator,
        T instance)
    {
        var result = await validator.ValidateAsync(instance);

        return result.IsValid
            ? instance
            : Failure<T>(result.ToErrors());
    }
}

public static class ValidationErrorMapper
{
    public static Error[] ToErrors(this FluentValidation.Results.ValidationResult result)
    {
        return result.Errors.Select(e =>
            new ValidationError(
                e.ErrorCode,
                e.ErrorMessage)
                .WithMetadata("property", e.PropertyName)).ToArray();
    }
}
