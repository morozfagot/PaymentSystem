namespace PaymentSystem.Shared.Domain;

public class Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("NullValue", "A null value was provided", ErrorType.Failure);

    private protected Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Authentication(string code, string description) =>
        new(code, description, ErrorType.Authentication);

    public static Error Authorization(string code, string description) =>
        new(code, description, ErrorType.Authorization);

    public static Error ResourceLimitExceeded(string code, string description) =>
        new(code, description, ErrorType.ResourceLimitExceeded);

    public static Error InternalServerError(string code, string description) =>
        new(code, description, ErrorType.InternalServerError);

    public static Error UnsupportedMediaType(string code, string description) =>
        new(code, description, ErrorType.UnsupportedMediaType);

    public static Error Unavailable(string code, string description) =>
        new(code, description, ErrorType.Unavailable);

    public static implicit operator Result(Error error) => Result.Failure(error);

    public Result ToResult() => Result.Failure(this);
}