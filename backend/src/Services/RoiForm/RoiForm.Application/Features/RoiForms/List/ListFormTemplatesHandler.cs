using MediatR;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.RoiForms.Abstractions;
using RoiForm.Application.Features.RoiForms.Dtos;

namespace RoiForm.Application.Features.RoiForms.List;

public class ListFormTemplatesHandler(IFormTemplateReadQueries queries)
    : IRequestHandler<ListFormTemplatesQuery, PagedResult<FormTemplateSummaryDto>>
{
    public Task<PagedResult<FormTemplateSummaryDto>> Handle(ListFormTemplatesQuery request, CancellationToken cancellationToken)
        => queries.ListAsync(request.Name, request.SortBy, request.SortDirection, request.Page, request.PageSize, cancellationToken);
}
