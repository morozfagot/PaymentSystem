namespace PaymentSystem.Modules.Payments.Domain.Operations;

/// <summary>
/// Value Object для хранения истории переходов статусов операции (state machine audit log).
/// Immutable, идентифицируется по значению полей.
/// </summary>
public sealed record OperationTransition
{
    /// <summary>
    /// Auto-increment первичный ключ (технический, для EF Core).
    /// </summary>
    public int EventId { get; init; }

    /// <summary>
    /// Идентификатор операции.
    /// </summary>
    public string OperationId { get; init; } = string.Empty;

    /// <summary>
    /// Тип/целевой статус перехода (CREATED, PROCESSING, COMPLETED, REJECTED).
    /// </summary>
    public OperationStatus Type { get; init; }

    /// <summary>
    /// Исходный статус (может быть null для CREATE).
    /// </summary>
    public OperationStatus? FromStatus { get; init; }

    /// <summary>
    /// Целевой статус.
    /// </summary>
    public OperationStatus ToStatus { get; init; }

    /// <summary>
    /// Описание перехода.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Время перехода (UTC).
    /// </summary>
    public DateTime OccurredAt { get; init; }
}