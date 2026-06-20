using FunctionalPrimitives.Monads.Results;
using FunctionalPrimitives.Monads.Results.Extensions;
using Domain.Common;
using RoiForm.Domain.FormManagement.Enums;
using RoiForm.Domain.FormManagement.Errors;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Domain.FormManagement.Entities;

public sealed class FormTemplate : AggregateRoot<Guid>, IAudit
{
    private readonly List<FormField> _formFields = [];

    // For EF Core
    private FormTemplate()
    {
    }

    private FormTemplate(
        string name,
        string title,
        RoiFormStatus status,
        Formula formula,
        List<FormField> formFields)
    {
        Name = name;
        Title = title;
        Status = status;
        Formula = formula;
        _formFields = formFields;
    }

    public string Name { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public RoiFormStatus Status { get; private set; }
    public Formula Formula { get; private set; } = null!;
    public IReadOnlyList<FormField> FormFields => _formFields.AsReadOnly();
    public string CreatedBy { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Result<FormTemplate> Create(
        string name,
        string title,
        Formula formula,
        IEnumerable<FormField> fields)
    {
        var fieldsArr = fields.ToArray();
        foreach (var token in formula.PostfixNotation)
        {
            if (token.Type != TokenType.Identifier)
            {
                continue;
            }

            if (!fieldsArr.Any(x => string.Equals(x.Identifier, token.Value, StringComparison.Ordinal)))
            {
                // return FormManagementErrors.InvalidFormulaIdentifier(token.Value);
            }
        }

        return
            from n in name
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.NameCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.NameTooLong())
            from t in title
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.TitleCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.TitleTooLong())
            from f in fieldsArr
                .Ensure(x => x.Length != 0, FormManagementErrors.FieldsCannotBeEmpty())
            select new FormTemplate(n, t, RoiFormStatus.Draft, formula, [.. f]);
    }

    public void SetCreatedInfo(string createdBy, DateTime createdAt)
    {
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public void SetUpdatedInfo(string? updatedBy, DateTime? updatedAt)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }

    public UnitResult UpdateFormula(Formula formula)
    {
        Formula = formula;

        return Unit.Value;
    }

    public UnitResult Rename(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Update(string title, Formula formula, IEnumerable<FormField> fields)
    {
        Title = title;
        Formula = formula;
        _formFields.Clear();
        _formFields.AddRange(fields);
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Publish()
    {
        if (Status != RoiFormStatus.Draft)
            return FormManagementErrors.CannotPublish(Status);

        Status = RoiFormStatus.Published;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Unpublish()
    {
        if (Status != RoiFormStatus.Published)
            return FormManagementErrors.CannotUnpublish(Status);

        Status = RoiFormStatus.Draft;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Archive()
    {
        if (Status != RoiFormStatus.Published)
            return FormManagementErrors.CannotArchive(Status);

        Status = RoiFormStatus.Archived;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }
}
