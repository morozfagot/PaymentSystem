using Microsoft.EntityFrameworkCore;
using PaymentSystem.Modules.Payments.Domain.Operations;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database;

/// <summary>
/// Репозиторий для платёжных операций на EF Core.
/// </summary>
internal sealed class OperationRepository : IOperationRepository
{
    private readonly PaymentsDbContext _dbContext;

    public OperationRepository(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Operation?> GetByIdAsync(string operationId, CancellationToken ct = default)
    {
        return await _dbContext.Operations
            .Include(o => o.Transitions)
            .FirstOrDefaultAsync(o => o.OperationId == operationId, ct);
    }

    public async Task AddAsync(Operation operation, CancellationToken ct = default)
    {
        await _dbContext.Operations.AddAsync(operation, ct);
    }
}