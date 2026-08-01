using System.Data.Common;
using Dapper;
using PaymentSystem.Modules.Payments.Domain.Operations.Events;
using PaymentSystem.Shared.Application.Data;
using PaymentSystem.Shared.Application.Exceptions;
using PaymentSystem.Shared.Application.Messaging;
using PaymentSystem.Shared.Domain;
using MediatR;

namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

/// <summary>
/// Обработчик доменного события OperationSubmittedDomainEvent.
/// Читает try_count из outbox, инкрементирует и отправляет команду.
/// При ошибке провайдера выбрасывает исключение,
/// чтобы ProcessOutboxJob сделал retry.
/// </summary>
internal sealed class SubmitOperationToProviderDomainEventHandler(
    ISender sender,
    IDbConnectionFactory dbConnectionFactory)
    : DomainEventHandler<OperationSubmittedDomainEvent>
{
    public override async Task Handle(
        OperationSubmittedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        int attemptNumber = await GetAttemptNumberAsync(domainEvent.OperationId);

        var command = new SubmitOperationToProviderCommand(
            domainEvent.OperationId,
            attemptNumber);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            throw new PaymentSystemException(nameof(SubmitOperationToProviderCommand));
        }
    }

    private async Task<int> GetAttemptNumberAsync(string operationId)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT COALESCE(try_count, 0) + 1
            FROM outbox_messages
            WHERE content LIKE @Pattern
            ORDER BY occurred_on_utc DESC
            LIMIT 1
            """;

        int? result = await connection.QuerySingleOrDefaultAsync<int?>(
            sql,
            new { Pattern = $"%{operationId}%" });

        return result ?? 1;
    }
}
