using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Domain.Operations;

/// <summary>
/// Aggregate root платёжной операции.
/// Управляет жизненным циклом: CREATED → PROCESSING → COMPLETED / REJECTED.
/// </summary>
public sealed class Operation : Entity
{
    private readonly List<OperationTransition> _transitions = [];

    private Operation(
        string operationId,
        decimal amount,
        string currency,
        string description)
        : base(Guid.NewGuid())
    {
        OperationId = operationId;
        Amount = amount;
        Currency = currency;
        Description = description;
        Status = OperationStatus.CREATED;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Пустой конструктор для EF Core.
    /// </summary>
    private Operation()
    {
    }

    /// <summary>
    /// Бизнес-идентификатор операции (передаётся извне).
    /// </summary>
    public string OperationId { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public OperationStatus Status { get; private set; }

    /// <summary>
    /// Идентификатор платежа в платёжной системе (заполняется из квитанции).
    /// </summary>
    public string? ProviderPaymentId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// История переходов статусов (state machine audit log).
    /// </summary>
    public IReadOnlyCollection<OperationTransition> Transitions => _transitions.AsReadOnly();

    /// <summary>
    /// Создаёт новую операцию в статусе CREATED.
    /// </summary>
    public static Result<Operation> Create(
        string operationId,
        decimal amount,
        string currency,
        string description,
        DateTime createdAt)
    {
        var operation = new Operation(operationId, amount, currency, description);

        operation.AddTransition(
            OperationStatus.CREATED, null, OperationStatus.CREATED,
            "Operation created", createdAt);

        operation.RaiseDomainEvent(new Events.OperationCreatedDomainEvent(
            operationId, amount, currency, description));

        return Result.Success(operation);
    }

    /// <summary>
    /// Отправляет операцию в платёжную систему.
    /// Если статус CREATED — переводит в PROCESSING.
    /// Если статус COMPLETED или REJECTED — возвращает Success (200, идемпотентность).
    /// </summary>
    public Result<bool> Submit(DateTime occurredAt)
    {
        if (Status == OperationStatus.CREATED)
        {
            Status = OperationStatus.PROCESSING;
            UpdatedAt = occurredAt;

            AddTransition(
                OperationStatus.PROCESSING, OperationStatus.CREATED, OperationStatus.PROCESSING,
                "Operation submitted for processing", occurredAt);

            RaiseDomainEvent(new Events.OperationSubmittedDomainEvent(OperationId));

            // Статус изменён: CREATED → PROCESSING
            return Result.Success(true);
        }

        // COMPLETED или REJECTED — идемпотентный повтор, статус не изменён
        AddTransition(
            Status, Status, Status,
            "Repeat submit attempt", occurredAt);

        return Result.Success(false);
    }

    /// <summary>
    /// Обрабатывает callback-квитанцию от платёжной системы.
    /// </summary>
    public Result Receipt(
        string providerPaymentId,
        OperationStatus status,
        string message,
        DateTime paidAt)
    {
        // Уже в терминальном статусе
        if (Status is OperationStatus.COMPLETED or OperationStatus.REJECTED)
        {
            // Несовпадающий providerPaymentId после установления связи — 409
            if (ProviderPaymentId != providerPaymentId)
            {
                return Result.Failure(OperationErrors.WrongProviderPaymentId(ProviderPaymentId!, providerPaymentId));
            }

            // Повторная или поздняя квитанция — логируем игнорирование
            AddTransition(status, Status, Status,
                $"Ignored duplicate/late receipt: {message}", paidAt);

            return Result.Success();
        }

        // Квитанция для операции не в PROCESSING (CREATED) — ошибка 
        if (Status != OperationStatus.PROCESSING)
        {
            return Result.Failure(OperationErrors.InvalidStatus(OperationStatus.PROCESSING, Status));
        }

        // PROCESSING — меняем статус на полученный из квитанции
        ProviderPaymentId = providerPaymentId;
        Status = status;
        UpdatedAt = paidAt;

        AddTransition(status, Status, status, message, paidAt);

        RaiseDomainEvent(new Events.OperationReceiptProcessedDomainEvent(
            OperationId, providerPaymentId, status, message, paidAt));

        return Result.Success();
    }

    private OperationTransition AddTransition(
        OperationStatus type, OperationStatus? fromStatus, OperationStatus toStatus,
        string message, DateTime occurredAt)
    {
        var transition = new OperationTransition
        {
            EventId = _transitions.Count + 1,
            OperationId = OperationId,
            Type = type,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Message = message,
            OccurredAt = occurredAt,
            StateChanged = fromStatus != toStatus,
        };

        _transitions.Add(transition);
        return transition;
    }
}