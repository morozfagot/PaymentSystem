using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using PaymentSystem.Modules.Payments.Application.Operations.Abstractions;
using PaymentSystem.Shared.Domain;

namespace PaymentSystem.Modules.Payments.Infrastructure.Provider;

/// <summary>
/// HTTP-клиент для provider-simulator.
/// Перед запросом делает задержку (exponential backoff + jitter) на основе
/// номера попытки. Повторные попытки управляются ProcessOutboxJob.
/// Возвращает Result с конкретной ошибкой для каждого случая.
/// </summary>
internal sealed class ProviderSimulatorClient : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ProviderOptions _options;
    private readonly Random _jitterer = new();

    public ProviderSimulatorClient(HttpClient httpClient, IOptions<ProviderOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<Result> SubmitAsync(
        string operationId,
        decimal amount,
        string currency,
        int attemptNumber,
        CancellationToken cancellationToken = default)
    {
        // Exponential backoff + jitter перед запросом
        if (attemptNumber > 1)
        {
            await WaitBeforeAttempt(attemptNumber, cancellationToken);
        }

        var request = new SubmitPaymentRequest(
            operationId,
            amount.ToString("F2"),
            currency);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "payments")
        {
            Content = JsonContent.Create(request),
        };

        httpRequest.Headers.TryAddWithoutValidation("Idempotency-Key", operationId);
        httpRequest.Headers.TryAddWithoutValidation("X-Correlation-ID", operationId);

        try
        {
            HttpResponseMessage response = await _httpClient
                .SendAsync(httpRequest, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure(ProviderErrors.UnexpectedHttpStatus((int)response.StatusCode));
            }

            // 202 Accepted — парсим тело ответа
            SubmitPaymentResponse? paymentResponse =
                await response.Content
                    .ReadFromJsonAsync<SubmitPaymentResponse>(cancellationToken)
                    .ConfigureAwait(false);

            if (paymentResponse?.Status is not ("ACCEPTED" or "REJECTED"))
            {
                return Result.Failure(ProviderErrors.UnexpectedResponseStatus(paymentResponse?.Status ?? "null"));
            }

            if (string.IsNullOrWhiteSpace(paymentResponse.ProviderPaymentId))
            {
                return Result.Failure(ProviderErrors.MissingProviderPaymentId);
            }

            return Result.Success();
        }
        catch (HttpRequestException)
        {
            // Сетевая ошибка — операция остаётся PROCESSING
            return Result.Failure(ProviderErrors.SubmissionFailed);
        }
        catch (TaskCanceledException)
        {
            // Таймаут — операция остаётся PROCESSING
            return Result.Failure(ProviderErrors.SubmissionFailed);
        }
    }

    private async Task WaitBeforeAttempt(int attemptNumber, CancellationToken cancellationToken)
    {
        double delayMs = _options.BaseDelayMs * Math.Pow(2, attemptNumber - 2);
        double jitterMs = _jitterer.NextDouble() * 100;
        await Task.Delay(TimeSpan.FromMilliseconds(delayMs + jitterMs), cancellationToken);
    }

    private sealed record SubmitPaymentRequest(
        [property: JsonPropertyName("operationId")] string OperationId,
        [property: JsonPropertyName("amount")] string Amount,
        [property: JsonPropertyName("currency")] string Currency);

    private sealed record SubmitPaymentResponse(
        [property: JsonPropertyName("providerPaymentId")] string? ProviderPaymentId,
        [property: JsonPropertyName("status")] string? Status);
}