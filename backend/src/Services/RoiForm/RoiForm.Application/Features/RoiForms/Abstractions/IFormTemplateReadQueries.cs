using FunctionalPrimitives.Monads.Options;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.RoiForms.Abstractions;

public interface IFormTemplateReadQueries
{
    Task<Option<FormTemplateDto>> FindByIdAsync(Guid id, CancellationToken ct);

    Task<PagedResult<FormTemplateSummaryDto>> ListAsync(
        string? name,
        string? sortBy,
        SortDirection sortDirection,
        int page,
        int pageSize,
        CancellationToken ct);
}
