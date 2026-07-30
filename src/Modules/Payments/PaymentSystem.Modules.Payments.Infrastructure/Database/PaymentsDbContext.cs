using Microsoft.EntityFrameworkCore;
using PaymentSystem.Modules.Payments.Domain.Operations;
using PaymentSystem.Modules.Payments.Infrastructure.Database.EntityConfigurations;
using PaymentSystem.Shared.Infrastructure.Outbox;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database;

/// <summary>
/// Контекст базы данных для платёжного модуля.
/// </summary>
public sealed class PaymentsDbContext : DbContext
{
    /// <summary>
    /// Платёжные операции.
    /// </summary>
    public DbSet<Operation> Operations => Set<Operation>();

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");

        modelBuilder.ApplyConfiguration(new OperationConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
    }
}