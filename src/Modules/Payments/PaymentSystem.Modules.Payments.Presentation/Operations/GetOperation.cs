using PaymentSystem.Modules.Payments.Application.Operations.GetOperation;
using PaymentSystem.Shared.Domain;
using PaymentSystem.Shared.Presentation.Endpoints;
using PaymentSystem.Shared.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Modules.Payments.Presentation.Operations;

internal sealed class GetOperation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("operations/{id}", async (string id, ISender sender) =>
        {
            Result<OperationResponse> result = await sender.Send(new GetOperationQuery(id));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Operations);
    }
}