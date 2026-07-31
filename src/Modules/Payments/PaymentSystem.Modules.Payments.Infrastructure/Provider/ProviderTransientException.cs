using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Infrastructure.Provider;

/// <summary>
/// Исключение, сигнализирующее о временной ошибке провайдера.
/// ProcessOutboxJob ловит его и делает retry.
/// </summary>
internal sealed class ProviderTransientException : Exception
{
    public Error Error { get; }

    public ProviderTransientException(Error error)
        : base(error.Description)
    {
        Error = error;
    }
}