using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record Address
{
    public const int MIN_LENGTH = 1;
    public const int MAX_LENGTH = 100;

    public string City { get; private set; }

    public string Street { get; private set; }

    public string House { get; private set; }

    public string Apartment { get; private set; }
    private Address(string city, string street, string house, string apartment)
    {
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }

    public static Result<Address, Error> Create(string city, string street, string house, string apartment)
    {
        List<ErrorMessages> errors = [];

        var cityResult = ValidAttribute(city, "City");
        if (cityResult.IsFailure)
            errors.AddRange(cityResult.Error);

        var streetResult = ValidAttribute(street, "Street");
        if (streetResult.IsFailure)
            errors.AddRange(streetResult.Error);

        var houseResult = ValidAttribute(house, "House");
        if (houseResult.IsFailure)
            errors.AddRange(houseResult.Error);

        var apartmentResult = ValidAttribute(apartment, "Apartment");
        if (apartmentResult.IsFailure)
            errors.AddRange(apartmentResult.Error);

        if (errors.Count > 0)
            return Error.Validation([.. errors]);

        return new Address(cityResult.Value, streetResult.Value, houseResult.Value, apartmentResult.Value);
    }

    private static Result<string, List<ErrorMessages>> ValidAttribute(string attribute, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(attribute))
        {
            return new List<ErrorMessages>
            {
                new(
                    $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.is.required",
                    $"Не задан параметр {fieldName}",
                    fieldName
                ),
            };
        }

        var errors = new List<ErrorMessages>();

        var normalized = attribute.Trim();

        if (normalized.Length < MIN_LENGTH)
        {
            errors.Add(new ErrorMessages
            (
                $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.invalid_length",
                $"Адрес {fieldName} должен содержать минимум {MIN_LENGTH} символа",
                fieldName
            ));
        }

        if (normalized.Length > MAX_LENGTH)
        {
            errors.Add(new ErrorMessages
            (
                $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.too_long",
                $"Адрес {fieldName} не должен превышать {MAX_LENGTH} символов",
                fieldName
            ));
        }

        if (normalized.Any(c => char.IsControl(c)))
        {
            errors.Add(new ErrorMessages
            (
                $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.invalid_format",
                $"Адрес {fieldName} не должен содержать управляющих символов",
                fieldName
            ));
        }

        if (errors.Count > 0)
            return errors;

        return normalized;
    }
    public override string ToString() => $"{City}, {Street}, {House}, {Apartment}";
}