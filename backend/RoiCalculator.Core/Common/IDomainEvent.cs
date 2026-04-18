namespace RoiCalculator.Core.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
