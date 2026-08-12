using MediatR;
using RoiForm.Application.Common.Query;

namespace RoiForm.Application.Features.FormTemplates.List;

public class ListFormTemplatesHandler(IListFormTemplatesReader reader)
    : IRequestHandler<ListFormTemplatesQuery, PagedResult<FormTemplateSummaryResponse>>
{
    public Task<PagedResult<FormTemplateSummaryResponse>> Handle(
        ListFormTemplatesQuery request,
        CancellationToken cancellationToken) => reader.List(request, cancellationToken);
}
