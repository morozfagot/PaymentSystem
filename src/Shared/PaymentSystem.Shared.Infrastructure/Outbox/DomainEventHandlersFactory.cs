using System.Collections.Concurrent;
using System.Reflection;
using PaymentSystem.Shared.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentSystem.Shared.Infrastructure.Outbox;

public static class DomainEventHandlersFactory
{
    private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

    public static IEnumerable<IDomainEventHandler> GetHandlers(
        Type type,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        Type[] domainEventHandlerTypes = HandlersDictionary.GetOrAdd(
            $"{assembly.GetName().Name}{type.Name}",
            _ =>
            {
                Type[] domainEventHandlerTypes = assembly.GetTypes()
                    .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
                    .ToArray();

                return domainEventHandlerTypes;
            });

        foreach (Type domainEventHandlerType in domainEventHandlerTypes)
        {
            yield return (serviceProvider.GetRequiredService(domainEventHandlerType) as IDomainEventHandler)!;
        }
    }
}