using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;
using Shared;

namespace DirectoryService.Domain.ValueObjects;

public partial record Slug
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 100;

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Result<Slug, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("slug.is.required", "Требуется slug", "slug");

        List<ErrorMessages> errors = [];

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length < MIN_LENGTH || normalized.Length > MAX_LENGTH)
        {
            errors.Add(new ErrorMessages
            (
                "slug.invalid_length",
                $"slug (от {MIN_LENGTH} до {MAX_LENGTH} символов)",
                "slug"
            ));
        }

        if (!SlugPattern().IsMatch(normalized))
        {
            errors.Add(new ErrorMessages
            (
                "slug.invalid_format",
                "slug (только строчные латинские буквы, цифры и дефисы, " +
                "не начинается и не заканчивается дефисом)",
                "slug"
            ));
        }

        if (errors.Count > 0)
            return Error.Validation([.. errors]);

        return new Slug(normalized);
    }

    [GeneratedRegex(@"^[a-z0-9][a-z0-9-]*[a-z0-9]$",
        RegexOptions.NonBacktracking | RegexOptions.CultureInvariant)]
    private static partial Regex SlugPattern();

    public static implicit operator string(Slug departmentSlug) => departmentSlug.Value;
    public override string ToString() => Value;
}