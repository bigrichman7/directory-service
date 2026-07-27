using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Domain.ValueObjects;

public partial record Slug
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 100;

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Result<Slug, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsRequired("Slug");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length < MIN_LENGTH || normalized.Length > MAX_LENGTH)
        {
            return GeneralErrors.ValueIsInvalid($"slug (от {MIN_LENGTH} до {MAX_LENGTH} символов)");
        }

        if (!SlugPattern().IsMatch(normalized))
        {
            return GeneralErrors.ValueIsInvalid(
                "slug (только строчные латинские буквы, цифры и дефисы, " +
                "не начинается и не заканчивается дефисом)");
        }

        return new Slug(normalized);
    }

    [GeneratedRegex(@"^[a-z0-9][a-z0-9-]*[a-z0-9]$",
        RegexOptions.NonBacktracking | RegexOptions.CultureInvariant)]
    private static partial Regex SlugPattern();

    public static implicit operator string(Slug departmentSlug) => departmentSlug.Value;
    public override string ToString() => Value;
}