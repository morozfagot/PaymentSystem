using PaymentSystem.Modules.Payments.Domain.Operations;

namespace PaymentSystem.Modules.Payments.Application.Operations.CreateOperation;

public sealed record OperationResponse(
    string OperationId,
    decimal Amount,
    string Currency,
    string Description,
    OperationStatus Status,
    string? ProviderPaymentId);
