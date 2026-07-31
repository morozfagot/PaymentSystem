using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;
using PaymentSystem.Modules.Payments.Application.Operations.GetOperation;
using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.CreateOperation;

internal sealed class CreateOperationCommandHandler(
    IOperationRepository operationRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateOperationCommand, OperationResponse>
{
    public async Task<Result<OperationResponse>> Handle(
        CreateOperationCommand request,
        CancellationToken cancellationToken)
    {
        if (await operationRepository.GetByIdAsync(request.OperationId, cancellationToken) is not null)
        {
            return Result.Failure<OperationResponse>(OperationErrors.AlreadyCreated(request.OperationId));
        }

        Result<Operation> result = Operation.Create(
            request.OperationId,
            request.Amount,
            request.Currency,
            request.Description,
            DateTime.UtcNow);

        if (result.IsFailure)
        {
            return Result.Failure<OperationResponse>(result.Error);
        }

        Operation operation = result.Value;

        await operationRepository.AddAsync(operation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new OperationResponse(
            operation.OperationId,
            operation.Amount,
            operation.Currency,
            operation.Description,
            operation.Status,
            operation.ProviderPaymentId);
    }
}