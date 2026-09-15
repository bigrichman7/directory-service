using System.Text.Json.Serialization;

namespace Shared;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.NONE, invalidField: null);

    public string Code { get; }

    public string Message { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }

    public string? InvalidField { get; }

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidField)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error NotFound(string? code, string message)
        => new(code ?? "record.not.found", message, ErrorType.NOT_FOUND, invalidField: null);

    public static Error Validation(string? code, string message, string? invalidField = null)
        => new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);

    public static Error Conflict(string? code, string message, string? invalidField = null)
        => new(code ?? "value.is.conflict", message, ErrorType.CONFLICT, invalidField: invalidField);

    public static Error Failure(string? code, string message)
        => new(code ?? "failure", message, ErrorType.FAILURE, invalidField: null);

    public Failure ToFailure() => this;
}

public enum ErrorType
{
    NONE,
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
}
