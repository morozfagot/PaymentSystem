using PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Presentation.Endpoints;
using PaymentSystem.Shared.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Modules.Payments.Presentation.Operations;

internal sealed class SubmitOperation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("operations/{id}/submit", async (string id, ISender sender) =>
        {
            Result<SubmitOperationResponse> result = await sender.Send(new SubmitOperationCommand(id));

            return result.Match(
                value => value.StateChanged
                    ? Results.Accepted($"/operations/{id}", null)
                    : Results.Ok(new { operationId = id, status = "already submitted" }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Operations);
    }
}