using FunctionalPrimitives.Monads.Results;
using FunctionalPrimitives.Monads.Results.Extensions;
using Domain.Common;
using RoiForm.Domain.FormManagement.Enums;
using RoiForm.Domain.FormManagement.Errors;

namespace RoiForm.Domain.FormManagement.Entities;

public class FormField : Entity<Guid>
{
    public string Identifier { get; private set; }
    public string Label { get; private set; }
    public bool IsRequired { get; private set; }
    public FormFieldType Type { get; private set; }
    public decimal? Min { get; private set; }
    public decimal? Max { get; private set; }

    private FormField(string identifier, string label, FormFieldType type, decimal? min, decimal? max)
    {
        Identifier = identifier;
        Label = label;
        Type = type;
        Min = min;
        Max = max;
    }

    public static Result<FormField> Create(string identifier, string label, FormFieldType type, decimal? min = null, decimal? max = null)
    {
        return min > max
            ? FormManagementErrors.FieldMinCannotExceedMax()
            : from i in identifier
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.FieldIdentifierCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.FieldIdentifierTooLong())
            from l in label
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.FieldLabelCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.FieldLabelTooLong())
            select new FormField(i, l, type, min, max);
    }
}
