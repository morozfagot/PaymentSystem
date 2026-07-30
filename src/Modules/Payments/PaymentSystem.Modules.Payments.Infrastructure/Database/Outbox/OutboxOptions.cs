namespace PaymentSystem.Modules.Payments.Infrastructure.Database.Outbox;

/// <summary>
/// Настройки обработки Outbox-сообщений для модуля Payments.
/// </summary>
internal sealed class OutboxOptions
{
    /// <summary>
    /// Интервал опроса новых сообщений в секундах.
    /// </summary>
    public int IntervalInSeconds { get; set; } = 10;

    /// <summary>
    /// Размер батча за одну итерацию.
    /// </summary>
    public int BatchSize { get; set; } = 20;  //я повторюсь         services.Configure<OutboxOptions>(configuration.GetSection("Ticketing:Outbox"));
}