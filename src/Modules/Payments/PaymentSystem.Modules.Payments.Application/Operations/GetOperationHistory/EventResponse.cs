using PaymentSystem.Modules.Payments.Domain.Operations;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperationHistory;

public sealed record EventResponse(
    int EventId,
    string OperationId,
    OperationStatus Type,
    OperationStatus? FromStatus,
    OperationStatus ToStatus,
    string Message,
    DateTime OccurredAt,
    bool StateChanged);