using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;
using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

internal sealed class SubmitOperationToProviderCommandHandler(
    IOperationRepository operationRepository,
    IPaymentService paymentService)
    : ICommandHandler<SubmitOperationToProviderCommand>
{
    public async Task<Result> Handle(
        SubmitOperationToProviderCommand request,
        CancellationToken cancellationToken)
    {
        Operation? operation = await operationRepository.GetByIdAsync(request.OperationId, cancellationToken);
        if (operation is null)
        {
            return Result.Failure(OperationErrors.NotFound(request.OperationId));
        }

        // Вызов внешнего платёжного сервиса
        // Результат обрабатывается через callback (ProcessReceipt)
        return await paymentService.SubmitAsync(
            operation.OperationId,
            operation.Amount,
            operation.Currency,
            cancellationToken);
    }
}