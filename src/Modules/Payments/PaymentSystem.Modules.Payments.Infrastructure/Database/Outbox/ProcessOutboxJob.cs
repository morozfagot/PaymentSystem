using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentSystem.Modules.Payments.Application;
using PaymentSystem.Shared.Application.Data;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Infrastructure.Outbox;
using PaymentSystem.Shared.Infrastructure.Serialization;
using Newtonsoft.Json;
using Quartz;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database.Outbox;

/// <summary>
/// Quartz job для обработки Outbox-сообщений модуля Payments.
/// Использует Dapper для прямых SQL-запросов.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger) : IJob
{
    private const string ModuleName = "Payments";
    private const int MaxRetryAttempts = 4;

    public async Task Execute(IJobExecutionContext context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);
        }

        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    SerializerSettings.Instance)!;

                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
                    domainEvent.GetType(),
                    scope.ServiceProvider,
                    AssemblyReference.Assembly);

                foreach (IDomainEventHandler domainEventHandler in handlers)
                {
                    await domainEventHandler.Handle(domainEvent, context.CancellationToken);
                }
            }
            catch (Exception caughtException)
            {
                logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing outbox message {MessageId}",
                    ModuleName,
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        await transaction.CommitAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "{Module} - Completed processing {Count} outbox messages",
                ModuleName,
                outboxMessages.Count);
        }
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                 id AS {nameof(OutboxMessageResponse.Id)},
                 content AS {nameof(OutboxMessageResponse.Content)},
                 type AS {nameof(OutboxMessageResponse.Type)},
                 try_count AS {nameof(OutboxMessageResponse.TryCount)}
             FROM payments.outbox_messages
             WHERE processed_on_utc IS NULL
               AND (try_count IS NULL OR try_count < {MaxRetryAttempts})
             ORDER BY occurred_on_utc
             LIMIT {outboxOptions.Value.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<OutboxMessageResponse> messages = await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);

        return messages.ToList();
    }

    private static async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql =
            """
            UPDATE payments.outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = @Error,
                try_count = COALESCE(try_count, 0) + 1
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = exception is null ? DateTime.UtcNow : (DateTime?)null,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(
        Guid Id,
        string Content,
        string Type,
        int? TryCount);
}