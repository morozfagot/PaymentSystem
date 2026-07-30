using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.ProcessReceipt;

public sealed record ProcessReceiptCommand(
    string OperationId,
    string ProviderPaymentId,
    OperationStatus Status,
    string Message,
    DateTime PaidAt) : ICommand;