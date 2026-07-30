using FluentValidation;

namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

internal sealed class SubmitOperationCommandValidator : AbstractValidator<SubmitOperationCommand>
{
    public SubmitOperationCommandValidator()
    {
        RuleFor(c => c.OperationId).NotEmpty();
    }
}