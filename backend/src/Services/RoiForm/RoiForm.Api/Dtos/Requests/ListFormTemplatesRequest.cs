using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.RoiForms.List;

namespace RoiForm.Api.Dtos.Requests;

public record ListFormTemplatesRequest(
    string? Name = null,
    string? SortBy = null,
    SortDirection? SortDirection = null,
    int Page = 1,
    int PageSize = 10);

public static class ListFormTemplatesRequestMapper
{
    public static ListFormTemplatesQuery ToQuery(this ListFormTemplatesRequest request)
        => new(
            request.Name,
            request.SortBy,
            request.SortDirection ?? SortDirection.Descending,
            request.Page,
            request.PageSize);
}
