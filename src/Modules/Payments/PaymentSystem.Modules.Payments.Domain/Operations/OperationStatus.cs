namespace PaymentSystem.Modules.Payments.Domain.Operations;

public enum OperationStatus
{
    /// <summary>
    /// Операция создана, ожидает отправки в платёжную систему.
    /// </summary>
    CREATED = 0,

    /// <summary>
    /// Операция отправлена в платёжную систему, ожидается результат.
    /// </summary>
    PROCESSING = 1,

    /// <summary>
    /// Операция успешно завершена.
    /// </summary>
    COMPLETED = 2,

    /// <summary>
    /// Операция отклонена платёжной системой.
    /// </summary>
    REJECTED = 3,
}