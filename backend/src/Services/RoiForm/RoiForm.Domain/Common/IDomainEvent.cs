namespace RoiForm.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
