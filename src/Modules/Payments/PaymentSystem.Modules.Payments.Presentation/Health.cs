using PaymentSystem.Shared.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Modules.Payments.Presentation;

internal sealed class Health : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("health", () => Results.Ok(new { status = "healthy" }))
            .WithTags(Tags.Health);
    }
}