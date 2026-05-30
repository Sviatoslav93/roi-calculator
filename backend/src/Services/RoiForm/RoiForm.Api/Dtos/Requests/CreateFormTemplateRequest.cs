using System.ComponentModel.DataAnnotations;
using RoiForm.Application.Features.RoiForms.Create;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Api.Dtos.Requests;

public class CreateFormTemplateRequest
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; init; }

    [Required]
    [MaxLength(200)]
    public required string Title { get; init; }

    [Required]
    [MaxLength(4000)]
    public required string Formula { get; init; }

    public IReadOnlyList<CreateFormFieldRequest> FormFields { get; init; } = [];
}

public sealed class CreateFormFieldRequest
{
    [Required]
    [MaxLength(100)]
    public required string Key { get; init; }

    [Required]
    [MaxLength(200)]
    public required string Label { get; init; }

    [Required]
    public required FormFieldType Type { get; init; }

    public decimal? Min { get; init; }

    public decimal? Max { get; init; }

    public bool IsRequired { get; init; }

    [MaxLength(500)]
    public string? Placeholder { get; init; }

    [MaxLength(1000)]
    public string? HelpText { get; init; }

    [Range(0, int.MaxValue)]
    public int Order { get; init; }
}

public static class CreateFormTemplateRequestMapper
{
    public static CreateFormTemplateCommand ToCommand(this CreateFormTemplateRequest request)
        => new(
            request.Name,
            request.Title,
            request.Formula,
            [.. request.FormFields.Select(f => new FormFieldInput(f.Key, f.Label, f.Type, f.Min, f.Max, f.IsRequired, f.Placeholder, f.HelpText, f.Order))]);
}
