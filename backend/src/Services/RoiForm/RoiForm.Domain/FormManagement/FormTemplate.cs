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
        string key,
        string title,
        FormTemplateStatus templateStatus,
        Formula formula,
        List<FormField> formFields)
    {
        Key = key;
        Title = title;
        TemplateStatus = templateStatus;
        Formula = formula;
        _formFields = formFields;
    }

    public string Key { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public FormTemplateStatus TemplateStatus { get; private set; }
    public Formula Formula { get; private set; } = null!;
    public IReadOnlyList<FormField> FormFields => _formFields.AsReadOnly();
    public string CreatedBy { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Result<FormTemplate> Create(
        string key,
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

            if (!fieldsArr.Any(x => string.Equals(x.Key, token.Value, StringComparison.Ordinal)))
            {
                // return FormManagementErrors.InvalidFormulaIdentifier(token.Value);
            }
        }

        return
            from n in key
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.NameCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.NameTooLong())
            from t in title
                .Ensure(x => !string.IsNullOrEmpty(x), FormManagementErrors.TitleCannotBeEmpty())
                .Ensure(x => x.Length <= 200, FormManagementErrors.TitleTooLong())
            from f in fieldsArr
                .Ensure(x => x.Length != 0, FormManagementErrors.FieldsCannotBeEmpty())
            select new FormTemplate(n, t, FormTemplateStatus.Draft, formula, [.. f]);
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

    public UnitResult Update(
        string key,
        string title,
        Formula formula,
        IEnumerable<FormField> fields)
    {
        Key = key;
        Title = title;
        Formula = formula;

        _formFields.Clear();
        _formFields.AddRange(fields);

        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Publish()
    {
        if (TemplateStatus != FormTemplateStatus.Draft)
            return FormManagementErrors.CannotPublish(TemplateStatus);

        TemplateStatus = FormTemplateStatus.Published;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Unpublish()
    {
        if (TemplateStatus != FormTemplateStatus.Published)
            return FormManagementErrors.CannotUnpublish(TemplateStatus);

        TemplateStatus = FormTemplateStatus.Draft;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }

    public UnitResult Archive()
    {
        if (TemplateStatus != FormTemplateStatus.Published)
            return FormManagementErrors.CannotArchive(TemplateStatus);

        TemplateStatus = FormTemplateStatus.Archived;
        UpdatedAt = DateTime.UtcNow;

        return Unit.Value;
    }
}
