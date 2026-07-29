using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Domain.Operations.Events;

public sealed class OperationCreatedDomainEvent : DomainEvent
{
    public OperationCreatedDomainEvent(
        string operationId,
        decimal amount,
        string currency,
        string description)
    {
        OperationId = operationId;
        Amount = amount;
        Currency = currency;
        Description = description;
    }

    public string OperationId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public string Description { get; init; }
}