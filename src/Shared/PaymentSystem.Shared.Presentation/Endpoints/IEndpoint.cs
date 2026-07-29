using Microsoft.AspNetCore.Routing;

namespace PaymentSystem.Shared.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}