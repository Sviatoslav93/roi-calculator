using System.ComponentModel.DataAnnotations;
using RoiForm.Application.Features.RoiForms.Update;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Api.Dtos.Requests;

public class UpdateFormTemplateRequest
{

    [Required]
    [MaxLength(200)]
    public required string Title { get; init; }

    [Required]
    [MaxLength(200)]
    public required string Formula { get; init; }
    public IReadOnlyList<UpdateFormFieldRequest> FormFields { get; init; } = [];
}

public class UpdateFormFieldRequest
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public required FormFieldType Type { get; init; }
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
}

public static class UpdateFormTemplateMapper
{
    public static UpdateFormTemplateCommand ToUpdateFormTemplateCommand(this UpdateFormTemplateRequest request, Guid id)
        => new(
            id,
            request.Title,
            request.Formula,
            [.. request.FormFields.Select(f => new UpdateFormFieldInput(f.Key, f.Label, f.Type, f.Min, f.Max))]);
}
