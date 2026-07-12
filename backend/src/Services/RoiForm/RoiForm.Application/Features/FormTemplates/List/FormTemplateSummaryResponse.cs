// ReSharper disable NotAccessedPositionalProperty.Global
namespace RoiForm.Application.Features.FormTemplates.List;

public record FormTemplateSummaryResponse(
    Guid Id,
    string Name,
    string Title,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
