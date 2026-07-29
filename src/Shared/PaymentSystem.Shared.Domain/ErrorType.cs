namespace PaymentSystem.Shared.Domain;

public enum ErrorType
{
    NotFound = 0,
    Validation = 1,
    Conflict = 2,
    Failure = 3,
    Authentication = 4,
    Authorization = 5,
    ResourceLimitExceeded = 6,
    InternalServerError = 7,
    UnsupportedMediaType = 8,
    Unavailable = 9
}