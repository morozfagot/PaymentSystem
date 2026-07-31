namespace PaymentSystem.Modules.Payments.Infrastructure.Provider;

/// <summary>
/// Конфигурация HTTP-клиента для provider-simulator.
/// </summary>
public sealed class ProviderOptions
{
    /// <summary>
    /// Секция в appsettings.json.
    /// </summary>
    public const string SectionName = "Payments:Provider";

    /// <summary>
    /// Базовый URL провайдера (в Docker Compose http://provider-simulator:8081).
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Таймаут HTTP-запроса в секундах.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Количество повторных попыток при ошибках 503 и сетевых ошибках.
    /// </summary>
    public int RetryCount { get; init; } = 3;

    /// <summary>
    /// Базовая задержка между retry в миллисекундах (для exponential backoff).
    /// </summary>
    public int BaseDelayMs { get; init; } = 200;
}