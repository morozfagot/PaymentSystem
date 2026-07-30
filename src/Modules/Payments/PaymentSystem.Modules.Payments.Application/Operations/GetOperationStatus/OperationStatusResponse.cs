using PaymentSystem.Modules.Payments.Domain.Operations;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperationStatus;

public sealed record OperationStatusResponse(
    string OperationId,
    OperationStatus Status);