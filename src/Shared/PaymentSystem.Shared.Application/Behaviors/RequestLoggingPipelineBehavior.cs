using PaymentSystem.Shared.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace PaymentSystem.Shared.Application.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string moduleName = GetModuleName(typeof(TRequest).FullName!);
        string requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("Module", moduleName))
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Processing request {RequestName}", requestName);
            }

            try
            {
                TResponse result = await next();

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Completed request {RequestName}", requestName);
                }

                return result;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Processing request {RequestName} failed", requestName);

                throw;
            }
        }
    }

    private static string GetModuleName(string requestFullName)
    {
        ReadOnlySpan<char> span = requestFullName.AsSpan();
        int firstDot = span.IndexOf('.');
        if (firstDot == -1)
        {
            return string.Empty;
        }

        ReadOnlySpan<char> afterModule = span[(firstDot + 1)..];
        int secondDot = afterModule.IndexOf('.');
        if (secondDot == -1)
        {
            return string.Empty;
        }

        return afterModule[..secondDot].ToString();
    }
}