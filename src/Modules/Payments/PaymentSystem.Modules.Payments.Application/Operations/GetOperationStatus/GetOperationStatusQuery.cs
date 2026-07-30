using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperationStatus;

public sealed record GetOperationStatusQuery(string OperationId) : IQuery<OperationStatusResponse>;