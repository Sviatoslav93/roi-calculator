using FunctionalPrimitives.Errors;
using FunctionalPrimitives.Errors.Extensions;

namespace RoiForm.Application.Features.RoiForms;

public static class FormTemplateApplicationErrors
{
    public static Error FormNotFound(Guid id) =>
        new NotFoundError($"Form template with id '{id}' was not found.", "form-template.not-found")
            .WithMetadata("id", id);

    public static Error NameConflict(string name) =>
        new ConflictError($"A form template with name '{name}' already exists.", "form-template.name-conflict")
            .WithMetadata("name", name);
}
