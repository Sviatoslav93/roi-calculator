using FunctionalPrimitives.Errors;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Domain.FormManagement.Errors;

public static class FormManagementErrors
{
    public static Error NameCannotBeEmpty() =>
        new ValidationError("Name cannot be empty", "roi-form.name-cannot-be-empty");

    public static Error NameTooLong() =>
        new ValidationError("Name cannot exceed 200 characters", "roi-form.name-too-long");

    public static Error TitleCannotBeEmpty() =>
        new ValidationError("Title cannot be empty", "roi-form.title-cannot-be-empty");

    public static Error TitleTooLong() =>
        new ValidationError("Title cannot exceed 200 characters", "roi-form.title-too-long");

    public static Error FieldsCannotBeEmpty() =>
        new ValidationError("Form must have at least one field", "roi-form.fields-cannot-be-empty");

    public static Error CannotPublish(RoiFormStatus currentStatus) =>
        new InvalidStateError(
            $"Cannot publish a form with status '{currentStatus}'. Only Draft forms can be published.",
            "roi-form.cannot-publish");

    public static Error CannotUnpublish(RoiFormStatus currentStatus) =>
        new InvalidStateError(
            $"Cannot unpublish a form with status '{currentStatus}'. Only Published forms can be unpublished.",
            "roi-form.cannot-unpublish");

    public static Error CannotArchive(RoiFormStatus currentStatus) =>
        new InvalidStateError(
            $"Cannot archive a form with status '{currentStatus}'. Only Published forms can be archived.",
            "roi-form.cannot-archive");

    public static Error FieldIdentifierCannotBeEmpty() =>
        new ValidationError("Field identifier cannot be empty", "roi-form.field-identifier-cannot-be-empty");

    public static Error FieldIdentifierTooLong() =>
        new ValidationError("Field identifier cannot exceed 200 characters", "roi-form.field-identifier-too-long");

    public static Error FieldLabelCannotBeEmpty() =>
        new ValidationError("Field label cannot be empty", "roi-form.field-label-cannot-be-empty");

    public static Error FieldLabelTooLong() =>
        new ValidationError("Field label cannot exceed 200 characters", "roi-form.field-label-too-long");

    public static Error FieldMinCannotExceedMax() =>
        new ValidationError("Field min value cannot exceed max value", "roi-form.field-min-cannot-exceed-max");

    public static Error FormulaCannotBeEmpty() =>
        new ValidationError("Formula expression cannot be empty", "roi-form.formula-cannot-be-empty");

    public static Error FormulaInvalidCharacters() =>
        new ValidationError("Formula expression contains invalid characters", "roi-form.formula-invalid-characters");

    public static Error FormulaUnbalancedParentheses() =>
        new ValidationError("Formula expression has unbalanced parentheses", "roi-form.formula-unbalanced-parentheses");

    public static Error FormulaInvalidExpression() =>
        new ValidationError("Formula expression is invalid", "roi-form.formula-invalid-expression");

    public static Error FormulaUnknownOperand(string identifier) =>
        new ValidationError($"Unknown operand '{identifier}'", "roi-form.formula-unknown-operand");

    public static Error FormulaDivideByZero() =>
        new ValidationError("Formula contains division by zero", "roi-form.formula-divide-by-zero");
}
