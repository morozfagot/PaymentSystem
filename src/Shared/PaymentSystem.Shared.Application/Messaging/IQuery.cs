using MediatR;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Shared.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}