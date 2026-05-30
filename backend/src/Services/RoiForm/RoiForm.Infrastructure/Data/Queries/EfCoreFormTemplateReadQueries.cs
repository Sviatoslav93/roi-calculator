using System.Linq.Expressions;
using FunctionalPrimitives.Monads.Options;
using FunctionalPrimitives.Monads.Options.Extensions;
using Microsoft.EntityFrameworkCore;
using RoiForm.Application.Common.Query;
using RoiForm.Application.Features.RoiForms.Abstractions;
using RoiForm.Application.Features.RoiForms.Dtos;
using RoiForm.Domain.FormManagement.Entities;

namespace RoiForm.Infrastructure.Data.Queries;

public class EfCoreFormTemplateReadQueries(AppDbContext context) : IFormTemplateReadQueries
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<FormTemplate, object>>> SortColumns =
        new Dictionary<string, Expression<Func<FormTemplate, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = x => x.Name,
            ["createdAt"] = x => x.CreatedAt,
            ["status"] = x => x.Status,
        };

    public async Task<Option<FormTemplateDto>> FindByIdAsync(Guid id, CancellationToken ct)
    {
        var form = await context.FormTemplates
            .AsNoTracking()
            .Include(x => x.FormFields)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return form is null ? None<FormTemplateDto>() : form.ToDto();
    }

    public async Task<PagedResult<FormTemplateSummaryDto>> ListAsync(
        string? name,
        string? sortBy,
        SortDirection sortDirection,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = context.FormTemplates.AsNoTracking().AsQueryable();

        if (name is not null)
            query = query.Where(x => x.Name == name);

        var totalCount = await query.CountAsync(ct);

        var descending = sortDirection == SortDirection.Descending;
        var safePage = page < 1 ? 1 : page;
        var safePageSize = pageSize < 1 ? 10 : pageSize;

        var forms = await query
            .ApplySort(sortBy ?? "createdAt", descending, SortColumns)
            .ApplyPaging(safePage, safePageSize)
            .ToListAsync(ct);

        return new PagedResult<FormTemplateSummaryDto>(
            forms.Select(x => x.ToSummaryDto()).ToList(),
            totalCount,
            safePage,
            safePageSize);
    }
}
