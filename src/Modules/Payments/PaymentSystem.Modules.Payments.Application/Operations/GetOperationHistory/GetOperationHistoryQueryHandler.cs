using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperationHistory;

internal sealed class GetOperationHistoryQueryHandler(
    IOperationRepository operationRepository)
    : IQueryHandler<GetOperationHistoryQuery, List<EventResponse>>
{
    public async Task<Result<List<EventResponse>>> Handle(
        GetOperationHistoryQuery request,
        CancellationToken cancellationToken)
    {
        Operation? operation = await operationRepository.GetByIdAsync(request.OperationId, cancellationToken);
        if (operation is null)
        {
            return Result.Failure<List<EventResponse>>(OperationErrors.NotFound(request.OperationId));
        }

        List<EventResponse> events = operation.Transitions
            .Select(t => new EventResponse(
                t.EventId,
                t.OperationId,
                t.Type,
                t.FromStatus,
                t.ToStatus,
                t.Message,
                t.OccurredAt))
            .ToList();

        return events;
    }
}