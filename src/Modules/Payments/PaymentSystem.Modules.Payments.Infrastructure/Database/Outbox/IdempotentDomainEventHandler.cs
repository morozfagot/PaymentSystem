using Microsoft.EntityFrameworkCore;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Infrastructure.Outbox;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database.Outbox;

/// <summary>
/// Generic idempotent-декоратор для доменных событий.
/// Проверяет outbox_message_consumers — если событие уже обработано, пропускает.
/// </summary>
internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    DomainEventHandler<TDomainEvent> decorated,
    PaymentsDbContext dbContext)
    : DomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public override async Task Handle(
        TDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var consumer = new OutboxMessageConsumer(domainEvent.Id, decorated.GetType().Name);

        if (await ConsumerExistsAsync(consumer, cancellationToken))
        {
            return;
        }

        await decorated.Handle(domainEvent, cancellationToken);

        dbContext.Set<OutboxMessageConsumer>().Add(consumer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> ConsumerExistsAsync(
        OutboxMessageConsumer consumer,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Set<OutboxMessageConsumer>()
            .AnyAsync(
                c => c.OutboxMessageId == consumer.OutboxMessageId && c.Name == consumer.Name,
                cancellationToken);
    }
}