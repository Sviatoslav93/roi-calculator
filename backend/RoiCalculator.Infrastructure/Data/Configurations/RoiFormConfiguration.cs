using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoiCalculator.Core.Aggregates;

namespace RoiCalculator.Infrastructure.Data.Configurations;

public class RoiFormConfiguration : IEntityTypeConfiguration<RoiForm>
{
    public void Configure(EntityTypeBuilder<RoiForm> builder)
    {
        builder.ToTable("roi_forms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
