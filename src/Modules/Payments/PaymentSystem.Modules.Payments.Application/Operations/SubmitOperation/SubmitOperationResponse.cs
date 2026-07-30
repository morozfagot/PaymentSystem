namespace PaymentSystem.Modules.Payments.Application.Operations.SubmitOperation;

/// <summary>
/// Содержит флаг изменения статуса операции для определения HTTP-статуса ответа:
/// - StateChanged == true → 202 (Accepted, статус изменён CREATED → PROCESSING)
/// - StateChanged == false → 200 (OK, идемпотентный повтор, статус не изменился)
/// </summary>
public sealed record SubmitOperationResponse(bool StateChanged);