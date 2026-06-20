using Microsoft.EntityFrameworkCore;
using Domain.Common;
using RoiForm.Domain.FormManagement.Entities;

namespace RoiForm.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<FormTemplate> FormTemplates => Set<FormTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IAudit>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedInfo("system", now);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetUpdatedInfo("system", now);
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
