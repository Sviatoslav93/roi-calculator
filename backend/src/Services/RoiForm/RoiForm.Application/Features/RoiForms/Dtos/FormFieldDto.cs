using RoiForm.Domain.FormManagement.Entities;

namespace RoiForm.Application.Features.RoiForms.Dtos;

public class FormFieldDto
{
    public required string Label { get; init; }
    public required string Key { get; init; }
    public required string Type { get; init; }
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
}

public static class FormFieldDtoMapper
{
    public static FormFieldDto ToFieldDto(this FormField field)
    {
        return new FormFieldDto
        {
            Label = field.Label,
            Key = field.Identifier,
            Type = field.Type.ToString(),
            Min = field.Min,
            Max = field.Max,
        };
    }
}
