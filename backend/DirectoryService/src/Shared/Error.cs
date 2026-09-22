using System.Text.Json.Serialization;

namespace Shared;

public record ErrorMessages(
    string Code,
    string Message,
    string? InvalidField = null
);
public record Error
{
    public IReadOnlyList<ErrorMessages> Messages { get; } = [];


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }


    [JsonConstructor]
    private Error(IEnumerable<ErrorMessages> messages, ErrorType type)
    {
        Messages = [.. messages];
        Type = type;
    }

    public static Error NotFound(string? code, string message)
        => new([new ErrorMessages(code ?? "record.not.found", message, InvalidField: null)], ErrorType.NOT_FOUND);
    public static Error NotFound(params ErrorMessages[] messages)
        => new(messages, ErrorType.NOT_FOUND);

    public static Error Validation(string? code, string message, string? invalidField = null)
        => new([new ErrorMessages(code ?? "value.is.invalid", message, invalidField)], ErrorType.VALIDATION);
    public static Error Validation(params ErrorMessages[] messages)
        => new(messages, ErrorType.VALIDATION);

    public static Error Conflict(string? code, string message, string? invalidField = null)
        => new([new ErrorMessages(code ?? "value.is.conflict", message, invalidField)], ErrorType.CONFLICT);
    public static Error Conflict(params ErrorMessages[] messages)
        => new(messages, ErrorType.CONFLICT);

    public static Error Failure(string? code, string message)
        => new([new ErrorMessages(code ?? "failure", message, InvalidField: null)], ErrorType.FAILURE);

    public static Error Failure(params ErrorMessages[] messages)
        => new(messages, ErrorType.FAILURE);

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
