using System.ComponentModel.DataAnnotations;
using RoiForm.Domain.FormManagement.Enums;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RoiForm.Api.Endpoints.FormTemplates.Create;

public class CreateFormTemplateRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = null!;

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = null!;

    [Required]
    [MaxLength(4000)]
    public string Formula { get; init; } = null!;

    public IReadOnlyList<CreateFormFieldRequest> FormFields { get; init; } = [];
}

public sealed class CreateFormFieldRequest
{
    [Required]
    [MaxLength(100)]
    public string Key { get; init; } = null!;

    [Required]
    [MaxLength(200)]
    public string Label { get; init; } = null!;

    [Required]
    public FormFieldType Type { get; init; }

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
