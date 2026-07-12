using FunctionalPrimitives.Errors;
using FunctionalPrimitives.Errors.Extensions;

namespace RoiForm.Application.Features.FormTemplates;

public static class FormTemplateApplicationErrors
{
    public static Error NameConflict(string name) =>
        new ConflictError($"A form template with name '{name}' already exists.", "form-template.name-conflict")
            .WithMetadata("name", name);
}
