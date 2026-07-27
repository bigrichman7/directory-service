using CSharpFunctionalExtensions;
using TemplateService.Domain.Common.Errors;

namespace TemplateService.Domain.ValueObjects;

public sealed record Address
{
    public const int MIN_LENGTH = 3;
    public const int MAX_LENGTH = 500;

    public string Value { get; private set; }

    private Address(string value)
    {
        Value = value;
    }

    public static Result<Address, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("Address");

        var normalized = value.Trim();

        if (normalized.Length < MIN_LENGTH)
            return GeneralErrors.ValueIsInvalid($"Адрес локации должен содержать минимум {MIN_LENGTH} символа");

        if (normalized.Length > MAX_LENGTH)
            return GeneralErrors.ValueIsInvalid($"Адрес локации не должен превышать {MAX_LENGTH} символов");

        if (normalized.Any(c => char.IsControl(c)))
            return GeneralErrors.ValueIsInvalid("Адрес локации не должен содержать управляющих символов");

        return new Address(normalized);
    }

    public static implicit operator string(Address address) => address.Value;
    public override string ToString() => Value;
}