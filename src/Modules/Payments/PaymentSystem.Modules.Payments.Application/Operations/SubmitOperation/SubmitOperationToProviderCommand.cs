using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

public sealed record SubmitOperationToProviderCommand(string OperationId) : ICommand;