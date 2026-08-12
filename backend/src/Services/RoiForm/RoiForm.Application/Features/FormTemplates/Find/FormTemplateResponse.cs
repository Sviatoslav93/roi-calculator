// ReSharper disable All

namespace RoiForm.Application.Features.RoiForms.Dtos;

public record FormTemplateResponse(
    Guid Id,
    string Name,
    string Title,
    string Status,
    string Formula,
    IReadOnlyList<FormFieldResponse> FormFields,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record FormFieldResponse(
    Guid Id,
    string Label,
    string Key,
    string Type,
    decimal? Min,
    decimal? Max
);
