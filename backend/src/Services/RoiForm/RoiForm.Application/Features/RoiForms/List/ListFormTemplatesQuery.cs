using MediatR;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.RoiForms.List;

public record ListFormTemplatesQuery(
    string? Name,
    string? SortBy,
    SortDirection SortDirection,
    int Page,
    int PageSize) : IRequest<PagedResult<FormTemplateSummaryDto>>;
