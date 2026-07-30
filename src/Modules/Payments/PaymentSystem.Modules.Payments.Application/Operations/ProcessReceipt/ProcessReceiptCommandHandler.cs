using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;
using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.ProcessReceipt;

internal sealed class ProcessReceiptCommandHandler(
    IOperationRepository operationRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ProcessReceiptCommand>
{
    public async Task<Result> Handle(
        ProcessReceiptCommand request,
        CancellationToken cancellationToken)
    {
        Operation? operation = await operationRepository.GetByIdAsync(request.OperationId, cancellationToken);
        if (operation is null)
        {
            return Result.Failure(OperationErrors.NotFound(request.OperationId));
        }

        // Делегируем доменной логике: проверка providerPaymentId, финальный статус, игнор
        Result result = operation.Receipt(
            request.ProviderPaymentId,
            request.Status,
            request.Message,
            request.PaidAt);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}