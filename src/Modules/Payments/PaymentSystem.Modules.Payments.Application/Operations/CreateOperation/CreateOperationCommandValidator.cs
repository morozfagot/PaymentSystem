using FluentValidation;

namespace PaymentSystem.Modules.Payments.Application.Operations.CreateOperation;

internal sealed class CreateOperationCommandValidator : AbstractValidator<CreateOperationCommand>
{
    public CreateOperationCommandValidator()
    {
        RuleFor(c => c.OperationId).NotEmpty();
        RuleFor(c => c.Amount)
            .GreaterThan(0)
            .Must(amount => decimal.Round(amount, 2) == amount)
            .WithMessage("Amount must have at most 2 decimal places.");
        RuleFor(c => c.Currency)
            .NotEmpty()
            .Must(c => c == "RUB")
            .WithMessage("Currency must be 'RUB'.");
    }
}