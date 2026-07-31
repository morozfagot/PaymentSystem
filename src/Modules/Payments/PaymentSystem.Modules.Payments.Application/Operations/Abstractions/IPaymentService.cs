using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.Abstractions;

/// <summary>
/// Абстракция для вызова внешнего платёжного сервиса (провайдера).
/// Реализация в Infrastructure слое.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Отправляет операцию в платёжную систему.
    /// </summary>
    /// <param name="operationId">Идентификатор операции (Idempotency-Key).</param>
    /// <param name="amount">Сумма операции.</param>
    /// <param name="currency">Валюта операции.</param>
    /// <param name="attemptNumber">Номер попытки (1 — первая). Используется для exponential backoff + jitter.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<Result> SubmitAsync(
        string operationId, 
        decimal amount, 
        string currency,
        int attemptNumber,
        CancellationToken cancellationToken = default);
}