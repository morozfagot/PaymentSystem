using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaymentSystem.Modules.Payments.Infrastructure.Database;

namespace PaymentSystem.Modules.Payments.Infrastructure;

/// <summary>
/// Factory for EF Core design-time tools (migrations).
/// </summary>
internal sealed class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<PaymentsDbContext> optionsBuilder = new();

        string connectionString = args.Length > 0
            ? args[0]
            : "Data Source=/data/payments.db";

        optionsBuilder.UseSqlite(connectionString);

        return new PaymentsDbContext(optionsBuilder.Options);
    }
}