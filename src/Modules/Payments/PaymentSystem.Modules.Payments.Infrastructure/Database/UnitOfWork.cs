using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database;

/// <summary>
/// Реализация Unit of Work через EF Core SaveChangesAsync.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly PaymentsDbContext _dbContext;

    public UnitOfWork(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}