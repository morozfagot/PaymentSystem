namespace PaymentSystem.Shared.Application.Exceptions;

public sealed class PaymentSystemException : Exception
{
    public PaymentSystemException(string requestName, Exception? innerException = default)
        : base($"PaymentSystem exception for {requestName}", innerException)
    {
        RequestName = requestName;
    }

    public string RequestName { get; }
}