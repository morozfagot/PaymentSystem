using Microsoft.EntityFrameworkCore;
using PaymentSystem.Modules.Payments.Infrastructure.Database;

namespace PaymentSystem.Api;

/// <summary>
/// Автоматическое применение миграций EF Core при старте приложения.
/// </summary>
internal static class MigrationExtensions
{
    /// <summary>
    /// Применяет ожидающие миграции для <see cref="PaymentsDbContext"/>.
    /// </summary>
    public static void ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        using PaymentsDbContext context = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();

        context.Database.EnsureCreated();
    }
}