namespace RoiCalculator.Core.DomainEvents;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
