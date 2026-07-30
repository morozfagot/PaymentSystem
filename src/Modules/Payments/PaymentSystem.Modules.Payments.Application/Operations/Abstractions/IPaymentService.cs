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
    Task<Result> SubmitAsync(
        string operationId, 
        decimal amount, 
        string currency, 
        CancellationToken cancellationToken = default);
}