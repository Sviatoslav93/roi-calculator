using FunctionalPrimitives.Monads.Results;
using FunctionalPrimitives.Monads.Results.Extensions;
using Domain.Common;
using RoiForm.Domain.FormManagement.Enums;
using RoiForm.Domain.FormManagement.Errors;

namespace RoiForm.Domain.FormManagement.Entities;

public class FormField : Entity<Guid>
{
    public string Key { get; private set; }
    public string Label { get; private set; }
    public bool IsRequired { get; private set; }
    public FormFieldType Type { get; private set; }
    public decimal? Min { get; private set; }
    public decimal? Max { get; private set; }

    private FormField(
        string key,
        string label,
        bool isRequired,
        FormFieldType type,
        decimal? min,
        decimal? max)
    {
        Key = key;
        Label = label;
        IsRequired = isRequired;
        Type = type;
        Min = min;
        Max = max;
    }

    public static Result<FormField> Create(
        string key,
        string label,
        bool isRequired,
        FormFieldType type,
        decimal? min = null,
        decimal? max = null)
    {
        return min > max
            ? FormManagementErrors.FieldMinCannotExceedMax()
            : from i in key
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.FieldIdentifierCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.FieldIdentifierTooLong())
            from l in label
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.FieldLabelCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.FieldLabelTooLong())
            select new FormField(i, l, isRequired, type, min, max);
    }
}
