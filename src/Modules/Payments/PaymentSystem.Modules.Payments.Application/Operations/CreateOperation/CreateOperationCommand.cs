using PaymentSystem.Modules.Payments.Application.Operations.GetOperation;
using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.CreateOperation;

public sealed record CreateOperationCommand(
    string OperationId,
    decimal Amount,
    string Currency,
    string Description) : ICommand<OperationResponse>;