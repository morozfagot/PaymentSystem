using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Domain.Operations.Events;

public sealed class OperationReceiptProcessedDomainEvent : DomainEvent
{
    public OperationReceiptProcessedDomainEvent(
        string operationId,
        string providerPaymentId,
        OperationStatus status,
        string message,
        DateTime paidAt)
    {
        OperationId = operationId;
        ProviderPaymentId = providerPaymentId;
        Status = status;
        Message = message;
        PaidAt = paidAt;
    }

    public string OperationId { get; init; }
    public string ProviderPaymentId { get; init; }
    public OperationStatus Status { get; init; }
    public string Message { get; init; }
    public DateTime PaidAt { get; init; }
}