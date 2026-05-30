using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Infrastructure.Data.Configurations;

public class FormTemplateConfiguration : IEntityTypeConfiguration<FormTemplate>
{
    public void Configure(EntityTypeBuilder<FormTemplate> builder)
    {
        builder.ToTable("roi_form_templates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);

        builder.OwnsMany(x => x.FormFields, fb =>
        {
            fb.ToTable("roi_form_fields");
            fb.WithOwner().HasForeignKey("RoiFormId");
            fb.HasKey(f => f.Id);

            fb.Ignore(f => f.DomainEvents);

            fb.Property(f => f.Identifier).IsRequired().HasMaxLength(200);
            fb.Property(f => f.Label).IsRequired().HasMaxLength(200);
            fb.Property(f => f.IsRequired).IsRequired();
            fb.Property(f => f.Type).IsRequired().HasConversion<int>();
            fb.Property(f => f.Min).HasColumnType("decimal(18,6)");
            fb.Property(f => f.Max).HasColumnType("decimal(18,6)");
        });

        builder.Navigation(x => x.FormFields)
            .HasField("_formFields")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsOne(x => x.Formula, fb =>
        {
            fb.Property(f => f.Expression)
                .HasColumnName("FormulaExpression")
                .IsRequired()
                .HasMaxLength(4000);

            fb.Property(f => f.PostfixNotation)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Token>>(v, (JsonSerializerOptions?)null)!,
                    new ValueComparer<IReadOnlyList<Token>>(
                        (a, b) => a != null && b != null && a.SequenceEqual(b),
                        c => c.Aggregate(0, (hash, t) => HashCode.Combine(hash, t.GetHashCode())),
                        c => c.ToList()
                    )
                )
                .HasColumnType("jsonb")
                .HasColumnName("PostfixNotation");
        });
    }
}
