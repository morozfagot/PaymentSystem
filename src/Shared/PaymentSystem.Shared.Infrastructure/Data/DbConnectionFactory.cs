using System.Data.Common;
using PaymentSystem.Shared.Application.Data;
using Npgsql;

namespace PaymentSystem.Shared.Infrastructure.Data;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        return await dataSource.OpenConnectionAsync();
    }
}