using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Infrastructure.Data;

namespace RoiCalculator.Infrastructure.Repositories;

public class RoiFormRepository(AppDbContext context) : IRoiFormRepository
{
    public async Task<RoiForm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.RoiForms.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<RoiForm>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.RoiForms.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RoiForm>> FindAsync(Expression<Func<RoiForm, bool>> predicate, CancellationToken cancellationToken = default)
        => await context.RoiForms.Where(predicate).ToListAsync(cancellationToken);

    public async Task<RoiForm> AddAsync(RoiForm entity, CancellationToken cancellationToken = default)
    {
        await context.RoiForms.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(RoiForm entity, CancellationToken cancellationToken = default)
    {
        context.RoiForms.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RoiForm entity, CancellationToken cancellationToken = default)
    {
        context.RoiForms.Remove(entity);
        return Task.CompletedTask;
    }
}
