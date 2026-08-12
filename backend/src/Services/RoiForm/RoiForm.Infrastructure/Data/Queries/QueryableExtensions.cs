using System.Linq.Expressions;

namespace RoiForm.Infrastructure.Data.Queries;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        public IQueryable<T> ApplySort(string? sortBy,
            bool descending,
            IReadOnlyDictionary<string, Expression<Func<T, object>>> columnMap)
        {
            if (sortBy is null || !columnMap.TryGetValue(sortBy, out var keySelector))
                return query;

            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        public IQueryable<T> ApplyPaging(int page, int pageSize)
            => query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
