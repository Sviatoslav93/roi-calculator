// ReSharper disable NotAccessedPositionalProperty.Global

using MediatR;
using RoiForm.Application.Common.Query;

namespace RoiForm.Application.Features.FormTemplates.List;

public record ListFormTemplatesQuery(
    string? Name,
    string? SortBy,
    SortDirection SortDirection,
    int Page,
    int PageSize) : IRequest<PagedResult<FormTemplateSummaryResponse>>;
