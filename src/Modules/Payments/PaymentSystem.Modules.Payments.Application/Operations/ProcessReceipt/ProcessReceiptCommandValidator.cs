using FluentValidation;
using PaymentSystem.Modules.Payments.Domain.Operations;

namespace PaymentSystem.Modules.Payments.Application.Operations.ProcessReceipt;

internal sealed class ProcessReceiptCommandValidator : AbstractValidator<ProcessReceiptCommand>
{
    public ProcessReceiptCommandValidator()
    {
        RuleFor(c => c.OperationId).NotEmpty();
        RuleFor(c => c.ProviderPaymentId).NotEmpty();
        RuleFor(c => c.Status)
            .Must(s => s is OperationStatus.COMPLETED or OperationStatus.REJECTED)
            .WithMessage("Receipt status must be COMPLETED or REJECTED.");
        RuleFor(c => c.PaidAt).NotEmpty();
    }
}