using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

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

    public static Result<Name, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
         return GeneralErrors.ValueIsRequired("Name of Department");

        var normalized = value.Trim();

        if (normalized.Length < MIN_LENGTH)
            return GeneralErrors.ValueIsInvalid($"Название локации должно содержать минимум {MIN_LENGTH} символа");

        if (normalized.Length > MAX_LENGTH)
            return GeneralErrors.ValueIsInvalid($"Название локации не должно превышать {MAX_LENGTH} символов");

        if (normalized.Any(c => char.IsControl(c)))
            return GeneralErrors.ValueIsInvalid("Название локации не должно содержать управляющих символов");

        return new Name(normalized);
    }

    public static implicit operator string(Name name) => name.Value;
    public override string ToString() => Value;
}