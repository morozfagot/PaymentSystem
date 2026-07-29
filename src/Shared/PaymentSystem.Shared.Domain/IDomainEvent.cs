namespace PaymentSystem.Shared.Domain;

public interface IDomainEvent
{
    Guid Id { get; init; }

    DateTime OccurredOnUtc { get; init; }
}