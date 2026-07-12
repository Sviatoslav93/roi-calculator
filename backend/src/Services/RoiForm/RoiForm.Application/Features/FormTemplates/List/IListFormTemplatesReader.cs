using RoiForm.Application.Common.Query;

namespace RoiForm.Application.Features.FormTemplates.List;

public interface IListFormTemplatesReader
{
    Task<PagedResult<FormTemplateSummaryResponse>> List(
        ListFormTemplatesQuery query,
        CancellationToken ct);
}
