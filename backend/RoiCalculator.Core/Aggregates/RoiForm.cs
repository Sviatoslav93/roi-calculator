using RoiCalculator.Core.Common;

namespace RoiCalculator.Core.Aggregates;

public sealed class RoiForm : AggregateRoot<Guid>
{
    private RoiForm() { }

    public string Name { get; private set; } = default!;
    public RoiFormStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static RoiForm Create(string name)
    {
        var now = DateTimeOffset.UtcNow;
        return new RoiForm
        {
            Id = Guid.NewGuid(),
            Name = name,
            Status = RoiFormStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Publish()
    {
        if (Status != RoiFormStatus.Draft)
            throw new InvalidOperationException($"Cannot publish a form with status '{Status}'. Only Draft forms can be published.");

        Status = RoiFormStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Unpublish()
    {
        if (Status != RoiFormStatus.Published)
            throw new InvalidOperationException($"Cannot unpublish a form with status '{Status}'. Only Published forms can be unpublished.");

        Status = RoiFormStatus.Draft;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        if (Status != RoiFormStatus.Published)
            throw new InvalidOperationException($"Cannot archive a form with status '{Status}'. Only Published forms can be archived.");

        Status = RoiFormStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
