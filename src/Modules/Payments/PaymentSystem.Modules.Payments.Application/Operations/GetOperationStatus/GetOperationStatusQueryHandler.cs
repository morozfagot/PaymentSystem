using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperationStatus;

internal sealed class GetOperationStatusQueryHandler(
    IOperationRepository operationRepository)
    : IQueryHandler<GetOperationStatusQuery, OperationStatusResponse>
{
    public async Task<Result<OperationStatusResponse>> Handle(
        GetOperationStatusQuery request,
        CancellationToken cancellationToken)
    {
        Operation? operation = await operationRepository.GetByIdAsync(request.OperationId, cancellationToken);
        if (operation is null)
        {
            return Result.Failure<OperationStatusResponse>(OperationErrors.NotFound(request.OperationId));
        }

        return new OperationStatusResponse(
            operation.OperationId,
            operation.Status);
    }
}