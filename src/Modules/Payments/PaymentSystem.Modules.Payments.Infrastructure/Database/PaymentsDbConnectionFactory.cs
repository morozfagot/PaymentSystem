using System.Data.Common;
using Microsoft.Data.Sqlite;
using PaymentSystem.Shared.Application.Data;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database;

/// <summary>
/// Фабрика подключений к SQLite для модуля Payments.
/// </summary>
internal sealed class PaymentsDbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}