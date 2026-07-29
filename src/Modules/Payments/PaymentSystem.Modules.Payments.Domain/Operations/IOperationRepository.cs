namespace PaymentSystem.Modules.Payments.Domain.Operations;

/// <summary>
/// Репозиторий для управления платёжными операциями.
/// </summary>
public interface IOperationRepository
{
    /// <summary>
    /// Получает операцию по бизнес-идентификатору (OperationId).
    /// </summary>
    Task<Operation?> GetByIdAsync(string operationId, CancellationToken ct = default);

    /// <summary>
    /// Добавляет новую операцию.
    /// </summary>
    Task AddAsync(Operation operation, CancellationToken ct = default);

    /// <summary>
    /// Получает все операции в статусе PROCESSING (для фоновой отправки).
    /// </summary>
    Task<List<Operation>> GetProcessingOperationsAsync(CancellationToken ct = default);
}