using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RoiForm.Domain.FormManagement;
using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Infrastructure.Data;

namespace RoiForm.Infrastructure.Repositories;

public class FormTemplateRepository(AppDbContext context) : IFormTemplateRepository
{
    public Task<FormTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.FormTemplates
            .Include(x => x.FormFields)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FormTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.FormTemplates.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FormTemplate>> FindAsync(Expression<Func<FormTemplate, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await context.FormTemplates.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<FormTemplate> AddAsync(FormTemplate entity, CancellationToken cancellationToken = default)
    {
        await context.FormTemplates.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(FormTemplate entity, CancellationToken cancellationToken = default)
    {
        context.FormTemplates.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(FormTemplate entity, CancellationToken cancellationToken = default)
    {
        context.FormTemplates.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await context.FormTemplates
            .AnyAsync(x => x.Key == name && (excludeId == null || x.Id != excludeId), cancellationToken);
    }
}
