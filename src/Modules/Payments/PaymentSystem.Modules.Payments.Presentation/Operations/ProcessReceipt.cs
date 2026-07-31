using PaymentSystem.Modules.Payments.Application.Operations.ProcessReceipt;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Presentation.Endpoints;
using PaymentSystem.Shared.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Modules.Payments.Presentation.Operations;

internal sealed class ProcessReceipt : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("receipts", async (Request request, ISender sender) =>
        {
            var command = new ProcessReceiptCommand(
                request.OperationId,
                request.ProviderPaymentId,
                Enum.Parse<Domain.Operations.OperationStatus>(request.Result, ignoreCase: true),
                request.Message,
                request.OccurredAt);

            Result result = await sender.Send(command);

            return result.Match(
                () => Results.NoContent(),
                ApiResults.Problem);
        })
        .WithTags(Tags.Operations);
    }

    internal sealed class Request
    {
        public string ProviderPaymentId { get; init; } = string.Empty;
        public string OperationId { get; init; } = string.Empty;
        public string Result { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public DateTime OccurredAt { get; init; }
    }
}