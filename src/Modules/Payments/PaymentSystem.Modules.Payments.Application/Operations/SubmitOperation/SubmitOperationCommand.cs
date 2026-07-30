using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

public sealed record SubmitOperationCommand(string OperationId) : ICommand<SubmitOperationResponse>;