using System.Data.Common;

namespace PaymentSystem.Shared.Application.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}