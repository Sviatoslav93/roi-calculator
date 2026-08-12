using Microsoft.EntityFrameworkCore;
using RoiForm.Domain.FormManagement.Entities;
using RoiForm.Domain.FormManagement.Enums;
using RoiForm.Domain.FormManagement.ValueObjects;

namespace RoiForm.Infrastructure.Data;

public class DatabaseSeeder(AppDbContext context)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync(cancellationToken);

        if (await context.FormTemplates.AnyAsync(cancellationToken))
            return;

        foreach (var form in SeedForms())
            await context.FormTemplates.AddAsync(form, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<FormTemplate> SeedForms()
    {
        yield return Build(
            "ROI Calculator",
            "Calculate Return on Investment",
            "(revenue - cost) / cost * 100",
            [("revenue", "Revenue"), ("cost", "Cost")]);

        yield return Build(
            "Profit Margin",
            "Calculate Profit Margin Percentage",
            "(revenue - expenses) / revenue * 100",
            [("revenue", "Revenue"), ("expenses", "Expenses")]);

        yield return Build(
            "Customer Lifetime Value",
            "Calculate Customer Lifetime Value",
            "avg_purchase * frequency * lifetime",
            [
                ("avg_purchase", "Avg Purchase Value"),
                ("frequency", "Purchase Frequency"),
                ("lifetime", "Customer Lifetime"),
            ]);
    }

    private static FormTemplate Build(
        string name,
        string title,
        string expression,
        IEnumerable<(string key, string label)> fieldDefs)
    {
        var formula = Formula.Create(expression).Value;
        var fields = fieldDefs
            .Select(f => FormField.Create(f.key, f.label, true, FormFieldType.Number).Value)
            .ToList();

        return FormTemplate.Create(name, title, formula, fields).Value;
    }
}
