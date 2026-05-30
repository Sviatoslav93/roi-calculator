using Dapper;
using FunctionalPrimitives.Monads.Options;
using FunctionalPrimitives.Monads.Options.Extensions;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.RoiForms.Abstractions;
using RoiForm.Application.Features.RoiForms.Dtos;
using RoiForm.Domain.FormManagement.Enums;

namespace RoiForm.Infrastructure.Data.Queries;

public class DapperFormTemplateReadQueries(IDbConnectionFactory dbFactory) : IFormTemplateReadQueries
{
    private static readonly IReadOnlyDictionary<string, string> SortColumns =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = "\"Name\"",
            ["createdAt"] = "\"CreatedAt\"",
            ["status"] = "\"Status\"",
        };

    private sealed record FormTemplateRow(Guid Id, string Name, string Title, int Status,
        string FormulaExpression, DateTime CreatedAt, DateTime? UpdatedAt);

    private sealed record FieldRow(string Identifier, string Label, int Type, decimal? Min, decimal? Max);

    private sealed record SummaryRow(Guid Id, string Name, string Title, int Status,
        DateTime CreatedAt, DateTime? UpdatedAt);

    public async Task<Option<FormTemplateDto>> FindByIdAsync(Guid id, CancellationToken ct)
    {
        await using var connection = await dbFactory.OpenConnectionAsync(ct);

        var form = await connection.QueryFirstOrDefaultAsync<FormTemplateRow>(
            """
            SELECT "Id", "Name", "Title", "Status", "FormulaExpression", "CreatedAt", "UpdatedAt"
            FROM roi_form_templates
            WHERE "Id" = @Id
            """,
            new { Id = id });

        if (form is null)
            return None<FormTemplateDto>();

        var fields = await connection.QueryAsync<FieldRow>(
            """SELECT "Identifier", "Label", "Type", "Min", "Max" FROM roi_form_fields WHERE "RoiFormId" = @RoiFormId""",
            new { RoiFormId = id });

        return new FormTemplateDto
        {
            Id = form.Id,
            Name = form.Name,
            Title = form.Title,
            Status = ((RoiFormStatus)form.Status).ToString(),
            Formula = form.FormulaExpression,
            FormFields = fields.Select(f => new FormFieldDto
            {
                Key = f.Identifier,
                Label = f.Label,
                Type = ((FormFieldType)f.Type).ToString(),
                Min = f.Min,
                Max = f.Max,
            }).ToList(),
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt
        };
    }

    public async Task<PagedResult<FormTemplateSummaryDto>> ListAsync(
        string? name,
        string? sortBy,
        SortDirection sortDirection,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        await using var connection = await dbFactory.OpenConnectionAsync(ct);

        const string where = "WHERE (@Name IS NULL OR \"Name\" = @Name)";
        var sortCol = SortColumns.GetValueOrDefault(sortBy ?? "createdAt", "\"CreatedAt\"");
        var dir = sortDirection == SortDirection.Descending ? "DESC" : "ASC";
        var param = new { Name = name, PageSize = pageSize, Offset = (page - 1) * pageSize };

        var totalCount = await connection.ExecuteScalarAsync<int>(
            $"SELECT COUNT(*) FROM roi_form_templates {where}",
            param);

        var rows = await connection.QueryAsync<SummaryRow>(
            $"""
            SELECT "Id", "Name", "Title", "Status", "CreatedAt", "UpdatedAt"
            FROM roi_form_templates {where}
            ORDER BY {sortCol} {dir}
            LIMIT @PageSize OFFSET @Offset
            """,
            param);

        var summaries = rows.Select(r => new FormTemplateSummaryDto
        {
            Id = r.Id,
            Name = r.Name,
            Title = r.Title,
            Status = ((RoiFormStatus)r.Status).ToString(),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
        }).ToList();

        return new PagedResult<FormTemplateSummaryDto>(summaries, totalCount, page, pageSize);
    }
}
