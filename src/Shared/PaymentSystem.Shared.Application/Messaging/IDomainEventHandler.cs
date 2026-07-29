using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Shared.Application.Messaging;

public interface IDomainEventHandler
{
    Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}