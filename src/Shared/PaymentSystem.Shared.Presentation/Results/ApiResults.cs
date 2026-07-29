using PaymentSystem.Shared.Domain;
using Microsoft.AspNetCore.Http;

namespace PaymentSystem.Shared.Presentation.Results;

public static class ApiResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result));
    }

    private static string GetTitle(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => "Not Found",
            ErrorType.Validation => "Validation Failed",
            ErrorType.Conflict => "Conflict",
            ErrorType.Authentication => "Authentication Failed",
            ErrorType.Authorization => "Authorization Failed",
            ErrorType.ResourceLimitExceeded => "Resource Limit Exceeded",
            ErrorType.InternalServerError => "Internal Server Error",
            ErrorType.UnsupportedMediaType => "Unsupported Media Type",
            ErrorType.Unavailable => "Service Unavailable",
            _ => "Failure"
        };

    private static string GetDetail(Error error) =>
        error.Description;

    private static string GetType(ErrorType type) =>
        type switch
        {
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Authentication => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ErrorType.Authorization => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            ErrorType.ResourceLimitExceeded => "https://tools.ietf.org/html/rfc6585#section-6",
            ErrorType.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            ErrorType.UnsupportedMediaType => "https://tools.ietf.org/html/rfc7231#section-6.5.13",
            ErrorType.Unavailable => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

    private static int GetStatusCode(ErrorType type) =>
        type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Authentication => StatusCodes.Status401Unauthorized,
            ErrorType.Authorization => StatusCodes.Status403Forbidden,
            ErrorType.ResourceLimitExceeded => StatusCodes.Status429TooManyRequests,
            ErrorType.InternalServerError => StatusCodes.Status500InternalServerError,
            ErrorType.UnsupportedMediaType => StatusCodes.Status415UnsupportedMediaType,
            ErrorType.Unavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

    private static Dictionary<string, object?>? GetErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors.Select(e => new { e.Code, e.Description }) }
        };
    }
}