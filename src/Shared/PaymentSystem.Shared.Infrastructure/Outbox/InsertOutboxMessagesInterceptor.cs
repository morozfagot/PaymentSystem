using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using Serilog;

namespace PaymentSystem.Shared.Infrastructure.Outbox;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Log.Debug("InsertOutboxMessagesInterceptor: SavingChangesAsync called, Context is {ContextNull}", eventData.Context is null ? "null" : "not null");

        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Log.Debug("InsertOutboxMessagesInterceptor: SavingChanges (sync) called, Context is {ContextNull}", eventData.Context is null ? "null" : "not null");

        if (eventData.Context is not null)
        {
            InsertOutboxMessages(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    private static void InsertOutboxMessages(DbContext context)
    {
        var entityEntries = context.ChangeTracker.Entries<Entity>().ToList();
        Log.Debug("InsertOutboxMessages: Found {Count} Entity entries in ChangeTracker", entityEntries.Count);

        foreach (var entry in entityEntries)
        {
            Log.Debug("InsertOutboxMessages: Entry Entity type = {Type}, State = {State}",
                entry.Entity.GetType().Name, entry.State);
            Log.Debug("InsertOutboxMessages: DomainEvents count = {Count}",
                entry.Entity.DomainEvents.Count);
        }

        var outboxMessages = entityEntries
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = domainEvent.Id,
                OccurredOnUtc = domainEvent.OccurredOnUtc,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(domainEvent, SerializerSettings.Instance)
            })
            .ToList();

        Log.Debug("InsertOutboxMessages: Created {Count} OutboxMessage records", outboxMessages.Count);

        if (outboxMessages.Count > 0)
        {
            context.Set<OutboxMessage>().AddRange(outboxMessages);
            Log.Debug("InsertOutboxMessages: Added {Count} OutboxMessage(s) to context", outboxMessages.Count);
        }
    }
}