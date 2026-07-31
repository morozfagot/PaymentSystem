using PaymentSystem.Modules.Payments.Application.Operations.CreateOperation;
using PaymentSystem.Modules.Payments.Application.Operations.GetOperation;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Presentation.Endpoints;
using PaymentSystem.Shared.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Modules.Payments.Presentation.Operations;

internal sealed class CreateOperation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("operations", async (Request request, ISender sender) =>
        {
            var command = new CreateOperationCommand(
                request.OperationId,
                decimal.Parse(request.Amount),
                request.Currency,
                request.Description);

            Result<OperationResponse> result = await sender.Send(command);

            return result.Match(
                value => Results.Created($"/operations/{value.OperationId}", value),
                ApiResults.Problem);
        })
        .WithTags(Tags.Operations);
    }

    internal sealed class Request
    {
        public string OperationId { get; init; } = string.Empty;
        public string Amount { get; init; } = string.Empty;
        public string Currency { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
}