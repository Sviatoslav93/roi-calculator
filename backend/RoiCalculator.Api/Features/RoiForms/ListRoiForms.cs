using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoiCalculator.Api.Common.Query;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Infrastructure.Data;

namespace RoiCalculator.Api.Features.RoiForms;

public record ListRoiFormsQuery(
    string? Name,
    string? SortBy,
    SortDirection SortDirection,
    int Page,
    int PageSize) : IRequest<PagedResult<RoiFormDto>>;

public class ListRoiFormsHandler(AppDbContext context)
    : IRequestHandler<ListRoiFormsQuery, PagedResult<RoiFormDto>>
{
    private static readonly IReadOnlyDictionary<string, Expression<Func<RoiForm, object>>> SortColumns =
        new Dictionary<string, Expression<Func<RoiForm, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = x => x.Name,
            ["createdAt"] = x => (object)x.CreatedAt,
            ["status"] = x => (object)x.Status
        };

    public async Task<PagedResult<RoiFormDto>> Handle(ListRoiFormsQuery request, CancellationToken cancellationToken)
    {
        var query = context.RoiForms.AsNoTracking().AsQueryable();

        if (request.Name is not null)
            query = query.Where(x => x.Name == request.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var sortBy = request.SortBy ?? "createdAt";
        var descending = request.SortDirection == SortDirection.Descending;

        var entities = await query
            .ApplySort(sortBy, descending, SortColumns)
            .ApplyPaging(request.Page, request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<RoiFormDto>(
            entities.Select(x => x.ToDto()).ToList(),
            totalCount,
            request.Page,
            request.PageSize);
    }
}
