using PaymentSystem.Modules.Payments.Application.Operations.GetOperationHistory;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Presentation.Endpoints;
using PaymentSystem.Shared.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Modules.Payments.Presentation.Operations;

internal sealed class GetOperationEvents : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("operations/{id}/events", async (string id, ISender sender) =>
        {
            Result<List<EventResponse>> result = await sender.Send(new GetOperationHistoryQuery(id));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Operations);
    }
}