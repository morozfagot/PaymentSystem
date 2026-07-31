using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Domain.Operations.Events;

public sealed class OperationSubmittedDomainEvent : DomainEvent
{
    public OperationSubmittedDomainEvent(string operationId, int attemptNumber = 1)
    {
        OperationId = operationId;
        AttemptNumber = attemptNumber;
    }

    public string OperationId { get; init; }

    /// <summary>
    /// Номер попытки отправки (1 — первая). Заполняется ProcessOutboxJob из try_count
    /// и используется для exponential backoff + jitter перед HTTP-вызовом.
    /// </summary>
    public int AttemptNumber { get; set; }
}