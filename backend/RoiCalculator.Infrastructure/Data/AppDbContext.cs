using Microsoft.EntityFrameworkCore;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Core.Common;

namespace RoiCalculator.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<RoiForm> RoiForms => Set<RoiForm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
