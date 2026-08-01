# PaymentSystem

Сервис обработки платёжных операций с конечным автоматом состояний и асинхронным взаимодействием с внешним провайдером.

**Главный инвариант:** каждая операция проходит строгий жизненный цикл `CREATED → PROCESSING → COMPLETED / REJECTED`. Переходы между состояниями защищены бизнес-правилами и фиксируются в аудит-логе.

## Требования

- [Docker](https://docs.docker.com/get-docker/) (обязательно)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Быстрый старт

```bash
# Запуск сервиса и симулятора провайдера
docker compose up --build
```

После запуска:

- **API сервис:** http://localhost:8080
- **Provider Simulator:** http://localhost:8081

## API Reference

| Метод | Путь | Описание | Статусы ответа |
|-------|------|----------|----------------|
| `GET` | `/health` | Проверка работоспособности сервиса | `200 OK` |
| `POST` | `/operations` | Создание новой платёжной операции | `201 Created`, `409 Conflict` |
| `POST` | `/operations/{id}/submit` | Отправка операции в платёжную систему | `202 Accepted`, `200 OK`, `404 Not Found` |
| `GET` | `/operations/{id}` | Получение текущего статуса операции | `200 OK`, `404 Not Found` |
| `GET` | `/operations/{id}/events` | История переходов статусов операции | `200 OK`, `404 Not Found` |
| `POST` | `/receipts` | Callback-квитанция от платёжной системы | `204 No Content`, `404 Not Found`, `409 Conflict` |

### POST /operations — Создание операции

```json
{
  "operationId": "order-42",
  "amount": "1500.00",
  "currency": "RUB",
  "description": "Оплата заказа №42"
}
```

**Ответ:** `201 Created`

```json
{
  "operationId": "order-42",
  "amount": 1500.00,
  "currency": "RUB",
  "description": "Оплата заказа №42",
  "status": "CREATED",
  "providerPaymentId": null
}
```

### POST /operations/{id}/submit — Отправка в платёжную систему

**Ответ (первый вызов):** `202 Accepted` — статус изменён на `PROCESSING`.

**Ответ (повторный вызов):** `200 OK`

```json
{
  "operationId": "order-42",
  "status": "already submitted"
}
```

### GET /operations/{id} — Получение статуса

**Ответ:** `200 OK`

```json
{
  "operationId": "order-42",
  "amount": 1500.00,
  "currency": "RUB",
  "description": "Оплата заказа №42",
  "status": "PROCESSING",
  "providerPaymentId": null
}
```

### GET /operations/{id}/events — История переходов

**Ответ:** `200 OK`

```json
[
  {
    "eventId": 1,
    "operationId": "order-42",
    "type": "CREATED",
    "fromStatus": null,
    "toStatus": "CREATED",
    "message": "Operation created",
    "occurredAt": "2026-08-01T07:00:00Z",
    "stateChanged": true
  },
  {
    "eventId": 2,
    "operationId": "order-42",
    "type": "PROCESSING",
    "fromStatus": "CREATED",
    "toStatus": "PROCESSING",
    "message": "Operation submitted for processing",
    "occurredAt": "2026-08-01T07:01:00Z",
    "stateChanged": true
  }
]
```

### POST /receipts — Callback от провайдера

```json
{
  "operationId": "order-42",
  "providerPaymentId": "prov-pay-123",
  "result": "COMPLETED",
  "message": "Payment successful",
  "occurredAt": "2026-08-01T07:02:00Z"
}
```

**Ответ:** `204 No Content`

## Полный сквозной сценарий

```bash
# 1. Health check
curl -s http://localhost:8080/health

# 2. Создание операции
curl -s -X POST http://localhost:8080/operations \
  -H "Content-Type: application/json" \
  -d '{
    "operationId": "order-42",
    "amount": "1500.00",
    "currency": "RUB",
    "description": "Оплата заказа №42"
  }'

# 3. Submit операции (отправка в платёжную систему)
curl -s -X POST http://localhost:8080/operations/order-42/submit

# 4. Проверка статуса (должен быть PROCESSING)
curl -s http://localhost:8080/operations/order-42

# 5. Проверка истории
curl -s http://localhost:8080/operations/order-42/events

# 6. Отправка callback (COMPLETED) — симуляция ответа провайдера
curl -s -X POST http://localhost:8080/receipts \
  -H "Content-Type: application/json" \
  -d '{
    "operationId": "order-42",
    "providerPaymentId": "prov-pay-123",
    "result": "COMPLETED",
    "message": "Payment successful",
    "occurredAt": "2026-08-01T07:02:00Z"
  }'

# 7. Проверка финального статуса (должен быть COMPLETED)
curl -s http://localhost:8080/operations/order-42
```

**Ожидаемый финальный ответ:**

```json
{
  "operationId": "order-42",
  "amount": 1500.00,
  "currency": "RUB",
  "description": "Оплата заказа №42",
  "status": "COMPLETED",
  "providerPaymentId": "prov-pay-123"
}
```

## Архитектура

Проект реализован как **микросервис** с разделением на функциональные слои:

```
src/
├── Api/PaymentSystem.Api          # ASP.NET Minimal API хост
├── Modules/Payments/
│   ├── Domain                     # Бизнес-логика, агрегат Operation, value objects
│   ├── Application                # CQRS команды/запросы, валидация, порты
│   ├── Infrastructure             # Persistence (SQLite), HTTP-клиент провайдера, Outbox
│   └── Presentation               # Minimal API эндпоинты
└── Shared/
    ├── Domain                     # Базовые типы: Entity, ValueObject, Result, Error
    ├── Application                # Абстракции CQRS, pipeline behaviors
    ├── Infrastructure             # DbConnectionFactory, Outbox, DateTimeProvider
    └── Presentation               # Endpoint-маршрутизация, ApiResults
```

### Ключевые компоненты

| Компонент | Назначение |
|-----------|------------|
| **Operation** (Domain) | Агрегат с конечным автоматом состояний. Управляет переходами `CREATED → PROCESSING → COMPLETED/REJECTED`. |
| **CQRS** (Application) | Команды (`CreateOperation`, `SubmitOperation`, `ProcessReceipt`) и запросы (`GetOperation`, `GetOperationHistory`). |
| **Outbox Pattern** (Infrastructure) | Надёжная доставка доменных событий через Quartz job. Сообщения сохраняются в БД в той же транзакции, что и агрегат. |
| **Provider Simulator Client** (Infrastructure) | HTTP-клиент к внешнему провайдеру с exponential backoff + jitter. |
| **Pipeline Behaviors** (Shared.Application) | Сквозная обработка: логирование, валидация, исключения. |

### Жизненный цикл операции

```
         CREATED
            │
            │ POST /operations/{id}/submit
            ▼
        PROCESSING
            │
            │ POST /receipts (callback от провайдера)
            ├──────────────────┬──────────────────┐
            ▼                  ▼                  ▼
        COMPLETED          REJECTED        PROCESSING (retry)
```

## Примечания

### Идемпотентность

- **Создание операции:** повторный `POST /operations` с тем же `operationId` возвращает `409 Conflict`.
- **Submit операции:** повторный `POST /operations/{id}/submit` возвращает `200 OK` (статус не меняется).
- **Callback:** повторная квитанция с тем же `providerPaymentId` логируется и возвращает `204 No Content`.
- **Provider Simulator:** использует заголовок `Idempotency-Key` (значение = `operationId`).

### Обработка ошибок

- **Валидация:** `ValidationPipelineBehavior` проверяет команды перед обработкой.
- **Бизнес-ошибки:** возвращаются через `Result<T>` с типизированными кодами ошибок.
- **Неожиданные исключения:** `ExceptionHandlingPipelineBehavior` и `GlobalExceptionHandler` преобразуют в `Problem Details`.
- **Сетевые ошибки провайдера:** `ProviderTransientException` — операция остаётся в `PROCESSING`, Outbox job повторяет попытку.

### Восстановление после сбоев

- **Outbox job** (`ProcessOutboxJob`) запускается каждые 10 секунд (настраивается в `appsettings.json`).
- **Max retry:** 4 попытки на одно outbox-сообщение.
- **Exponential backoff:** задержка между retry = `BaseDelayMs × 2^(attempt-2)` + jitter.
- **SQLite:** БД хранится в Docker volume `candidate-data:/data`.

### Статусы операций

| Статус | Описание |
|--------|----------|
| `CREATED` | Операция создана, ожидает отправки в платёжную систему |
| `PROCESSING` | Операция отправлена, ожидается callback |
| `COMPLETED` | Операция успешно завершена |
| `REJECTED` | Операция отклонена провайдером |