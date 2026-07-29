using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Shared.Application.Messaging;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler
    where TDomainEvent : IDomainEvent
{
    public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);

    async Task IDomainEventHandler.Handle(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is TDomainEvent typedDomainEvent)
        {
            await Handle(typedDomainEvent, cancellationToken);
        }
    }
}