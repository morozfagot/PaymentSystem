using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Domain.Operations;

public static class OperationErrors
{
    public static Error NotFound(string operationId) =>
        Error.NotFound(
            "Operation.NotFound",
            $"Operation with ID '{operationId}' was not found.");

    public static Error AlreadyCreated(string operationId) =>
        Error.Conflict(
            "Operation.AlreadyCreated",
            $"Operation with ID '{operationId}' already exists.");

    public static Error InvalidStatus(OperationStatus expected, OperationStatus actual) =>
        Error.Conflict(
            "Operation.InvalidStatus",
            $"Expected status '{expected}' but current status is '{actual}'.");

    public static Error WrongProviderPaymentId(string expected, string actual) =>
        Error.Conflict(
            "Operation.WrongProviderPaymentId",
            $"Expected provider payment ID '{expected}' but received '{actual}'.");
}