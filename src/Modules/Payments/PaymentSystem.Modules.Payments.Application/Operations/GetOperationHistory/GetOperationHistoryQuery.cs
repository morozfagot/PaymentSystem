using PaymentSystem.Shared.Application.Messaging;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperationHistory;

public sealed record GetOperationHistoryQuery(string OperationId) : IQuery<List<EventResponse>>;