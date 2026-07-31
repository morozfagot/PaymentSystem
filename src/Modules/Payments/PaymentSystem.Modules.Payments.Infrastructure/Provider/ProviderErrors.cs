using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Infrastructure.Provider;

/// <summary>
/// Ошибки, связанные с вызовом внешнего платёжного провайдера.
/// </summary>
public static class ProviderErrors
{
    /// <summary>
    /// Провайдер вернул HTTP-ошибку или недоступен (transient).
    /// </summary>
    public static readonly Error SubmissionFailed = Error.Failure(
        "Provider.SubmissionFailed",
        "Payment provider returned an error or is unavailable.");

    /// <summary>
    /// Провайдер вернул неожиданный HTTP-статус (transient).
    /// </summary>
    public static Error UnexpectedHttpStatus(int statusCode) => Error.Failure(
        "Provider.UnexpectedHttpStatus",
        $"Payment provider returned unexpected HTTP status: {statusCode}.");

    /// <summary>
    /// Провайдер вернул ответ без providerPaymentId (transient).
    /// </summary>
    public static readonly Error MissingProviderPaymentId = Error.Failure(
        "Provider.MissingProviderPaymentId",
        "Payment provider response is missing providerPaymentId.");

    /// <summary>
    /// Провайдер вернул неожиданный статус в теле ответа (transient).
    /// </summary>
    public static Error UnexpectedResponseStatus(string status) => Error.Failure(
        "Provider.UnexpectedResponseStatus",
        $"Payment provider returned unexpected response status: '{status}'.");
}