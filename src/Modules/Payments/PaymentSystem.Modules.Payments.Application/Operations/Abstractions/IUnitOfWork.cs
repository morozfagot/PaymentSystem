namespace PaymentSystem.Modules.Payments.Application.Operations.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}