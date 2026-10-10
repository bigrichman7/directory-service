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
         return Error.Validation("name.is.required", "Требуется название", "name");

        List<ErrorMessages> errors = [];

        var normalized = value.Trim();

        if (normalized.Length < MIN_LENGTH)
        {
            errors.Add(new ErrorMessages
            (
                "name.invalid_length",
                $"Название локации должно содержать минимум {MIN_LENGTH} символа",
                "name"
            ));
        }

        if (normalized.Length > MAX_LENGTH)
        {
            errors.Add(new ErrorMessages
            (
                "name.too_long",
                $"Название локации не должно превышать {MAX_LENGTH} символов",
                "name"
            ));
        }

        if (normalized.Any(c => char.IsControl(c)))
        {
            errors.Add(new ErrorMessages
            (
                "name.invalid_format",
                "Название локации не должно содержать управляющих символов",
                "name"
            ));
        }
        
        if (errors.Count > 0)
        {
            return Error.Validation([..errors]);
        }

        return new Name(normalized);
    }

    public static implicit operator string(Name name) => name.Value;
    public override string ToString() => Value;
}