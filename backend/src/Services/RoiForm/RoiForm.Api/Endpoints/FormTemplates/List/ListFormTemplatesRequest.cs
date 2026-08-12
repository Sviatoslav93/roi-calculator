using RoiForm.Application.Common.Query;

namespace RoiForm.Api.Dtos.Requests;

public class ListFormTemplatesRequest
{
    public string? Name { get; init; }
    public string? SortBy { get; init; }
    public SortDirection? SortDirection { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
