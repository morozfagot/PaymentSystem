using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;
using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Shared.Application.Clock;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

internal sealed class SubmitOperationCommandHandler(
    IOperationRepository operationRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<SubmitOperationCommand, SubmitOperationResponse>
{
    public async Task<Result<SubmitOperationResponse>> Handle(
        SubmitOperationCommand request,
        CancellationToken cancellationToken)
    {
        Operation? operation = await operationRepository.GetByIdAsync(request.OperationId, cancellationToken);
        if (operation is null)
        {
            return Result.Failure<SubmitOperationResponse>(OperationErrors.NotFound(request.OperationId));
        }

        // Атомарно: меняем статус CREATED → PROCESSING, сохраняем transition
        // Submit возвращает true если статус изменён, false если идемпотентный повтор
        Result<bool> submitResult = operation.Submit(dateTimeProvider.UtcNow);
        if (submitResult.IsFailure)
        {
            return Result.Failure<SubmitOperationResponse>(submitResult.Error);
        }

        // Сохраняем намерение ДО внешнего вызова (Outbox перехватит доменное событие)
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitOperationResponse(submitResult.Value);
    }
}