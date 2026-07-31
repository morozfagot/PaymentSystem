using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperation;

public sealed record GetOperationQuery(string OperationId) : IQuery<OperationResponse>;