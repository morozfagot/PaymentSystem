using MediatR;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Shared.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}