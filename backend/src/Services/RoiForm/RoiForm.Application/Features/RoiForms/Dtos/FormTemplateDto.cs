using RoiForm.Domain.FormManagement.Entities;

namespace RoiForm.Application.Features.RoiForms.Dtos;

public class FormTemplateDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public required string Formula { get; init; }
    public required IReadOnlyList<FormFieldDto> FormFields { get; init; } = [];
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}

public static class FormTemplateDtoMappings
{
    public static FormTemplateDto ToDto(this FormTemplate formTemplate)
    {
        return new FormTemplateDto
        {
            Id = formTemplate.Id,
            Name = formTemplate.Name,
            Title = formTemplate.Title,
            Status = formTemplate.Status.ToString(),
            Formula = formTemplate.Formula.Expression,
            FormFields = formTemplate.FormFields.Select(f => f.ToFieldDto()).ToList(),
            CreatedAt = formTemplate.CreatedAt,
            UpdatedAt = formTemplate.UpdatedAt
        };
    }
}
