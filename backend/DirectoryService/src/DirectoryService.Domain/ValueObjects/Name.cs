using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record Name
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 200;

    public string Value { get; private set; }

    private Name(string value)
    {
        Value = value;
    }

    public static Result<Name, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
         return Error.Validation("name.is.required", "Требуется название");

        var normalized = value.Trim();

        if (normalized.Length < MIN_LENGTH)
            return Error.Validation("name.invalid_length", $"Название локации должно содержать минимум {MIN_LENGTH} символа");

        if (normalized.Length > MAX_LENGTH)
            return Error.Validation("name.too_long", $"Название локации не должно превышать {MAX_LENGTH} символов");

        if (normalized.Any(c => char.IsControl(c)))
            return Error.Validation("name.invalid_format", "Название локации не должно содержать управляющих символов");

        return new Name(normalized);
    }

    public static implicit operator string(Name name) => name.Value;
    public override string ToString() => Value;
}