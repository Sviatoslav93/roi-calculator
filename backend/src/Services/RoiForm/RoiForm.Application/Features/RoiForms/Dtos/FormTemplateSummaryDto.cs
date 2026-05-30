using RoiForm.Domain.FormManagement.Entities;

namespace RoiForm.Application.Features.RoiForms.Dtos;

public class FormTemplateSummaryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Title { get; init; } = null!;
    public required string Status { get; init; } = null!;
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}

public static class FormTemplateSummaryDtoMapper
{
    public static FormTemplateSummaryDto ToSummaryDto(this FormTemplate formTemplate)
    {
        return new FormTemplateSummaryDto
        {
            Id = formTemplate.Id,
            Name = formTemplate.Name,
            Title = formTemplate.Title,
            Status = formTemplate.Status.ToString(),
            CreatedAt = formTemplate.CreatedAt,
            UpdatedAt = formTemplate.UpdatedAt
        };
    }
}
