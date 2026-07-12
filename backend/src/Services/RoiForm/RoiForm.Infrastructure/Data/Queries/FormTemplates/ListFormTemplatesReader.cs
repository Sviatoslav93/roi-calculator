using Dapper;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.FormTemplates.List;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Infrastructure.Data.Queries.FormTemplates;

public class ListFormTemplatesReader(IDbConnectionFactory dbFactory) : IListFormTemplatesReader
{
    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = "\"Key\"",
            ["createdAt"] = "\"CreatedAt\"",
            ["templateStatus"] = "\"TemplateStatus\"",
        };

    public async Task<PagedResult<FormTemplateSummaryResponse>> List(ListFormTemplatesQuery query, CancellationToken ct)
    {
        var (name, sortBy, sortDirection, page, pageSize ) = query;

        await using var connection = await dbFactory.OpenConnectionAsync(ct);

        const string where = "WHERE (@Key IS NULL OR \"Key\" = @Key)";
        var sortCol = SortColumns.GetValueOrDefault(sortBy ?? "createdAt", "\"CreatedAt\"");
        var dir = sortDirection == SortDirection.Descending ? "DESC" : "ASC";
        var param = new { Name = name, PageSize = pageSize, Offset = (page - 1) * pageSize };

        var totalCount = await connection.ExecuteScalarAsync<int>(
            $"SELECT COUNT(*) FROM roi_form_templates {where}",
            param);

        var rows = await connection.QueryAsync<FormTemplateSummaryResponse>(
            $"""
            SELECT Id, Key, Title, TemplateStatus, CreatedAt, UpdatedAt
            FROM roi_form_templates {where}
            ORDER BY {sortCol} {dir}
            LIMIT @PageSize OFFSET @Offset
            """,
            param);

        return new PagedResult<FormTemplateSummaryResponse>([..rows], totalCount, page, pageSize);
    }
}
