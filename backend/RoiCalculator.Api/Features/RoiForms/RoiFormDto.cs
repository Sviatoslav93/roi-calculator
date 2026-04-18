using RoiCalculator.Core.Aggregates;

namespace RoiCalculator.Api.Features.RoiForms;

public record RoiFormDto(
    Guid Id,
    string Name,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public static class RoiFormMappings
{
    public static RoiFormDto ToDto(this RoiForm form) =>
        new(form.Id, form.Name, form.Status.ToString(), form.CreatedAt, form.UpdatedAt);
}
