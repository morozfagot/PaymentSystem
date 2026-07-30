using PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;
using PaymentSystem.Modules.Payments.Domain.Operations.Events;
using PaymentSystem.Shared.Application.Messaging;
using MediatR;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database.Outbox;

/// <summary>
/// Обработчик доменного события OperationSubmittedDomainEvent.
/// Отправляет команду SubmitOperationToProviderCommand через ISender.
/// </summary>
internal sealed class SubmitOperationToProviderDomainEventHandler(
    ISender sender)
    : DomainEventHandler<OperationSubmittedDomainEvent>
{
    public override async Task Handle(
        OperationSubmittedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var command = new SubmitOperationToProviderCommand(domainEvent.OperationId);
        await sender.Send(command, cancellationToken);
    }
}