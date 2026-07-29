using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Domain.Operations.Events;

public sealed class OperationSubmittedDomainEvent : DomainEvent
{
    public OperationSubmittedDomainEvent(string operationId)
    {
        OperationId = operationId;
    }

    public string OperationId { get; init; }
}