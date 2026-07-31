using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.GetOperation;

internal sealed class GetOperationQueryHandler(
    IOperationRepository operationRepository)
    : IQueryHandler<GetOperationQuery, OperationResponse>
{
    public async Task<Result<OperationResponse>> Handle(
        GetOperationQuery request,
        CancellationToken cancellationToken)
    {
        Operation? operation = await operationRepository.GetByIdAsync(request.OperationId, cancellationToken);
        if (operation is null)
        {
            return Result.Failure<OperationResponse>(OperationErrors.NotFound(request.OperationId));
        }

        return new OperationResponse(
            operation.OperationId,
            operation.Amount,
            operation.Currency,
            operation.Description,
            operation.Status,
            operation.ProviderPaymentId);
    }
}