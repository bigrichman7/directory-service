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
        var cityResult = ValidAttribute(city, "City");
        if (cityResult.IsFailure)
            return cityResult.Error;
        var normalizedCity = cityResult.Value;

        var streetResult = ValidAttribute(street, "Street");
        if (streetResult.IsFailure)
            return streetResult.Error;
        var normalizedStreet = streetResult.Value;

        var houseResult = ValidAttribute(house, "House");
        if (houseResult.IsFailure)
            return houseResult.Error;
        var normalizedHouse = houseResult.Value;

        var apartmentResult = ValidAttribute(apartment, "Apartment");
        if (apartmentResult.IsFailure)
            return apartmentResult.Error;
        var normalizedApartment = apartmentResult.Value;

        return new Address(normalizedCity, normalizedStreet, normalizedHouse, normalizedApartment);
    }

    private static Result<string, Error> ValidAttribute(string attribute, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(attribute))
        {
            return Error.Validation($"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.is.required", $"Не задан параметр {fieldName}");
        }

        var normalized = attribute.Trim();

        if (normalized.Length < MIN_LENGTH)
            return Error.Validation(
                $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.invalid_length", 
                $"Адрес {fieldName} должен содержать минимум {MIN_LENGTH} символа");

        if (normalized.Length > MAX_LENGTH)
            return Error.Validation(
                $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.too_long", 
                $"Адрес {fieldName} не должен превышать {MAX_LENGTH} символов");

        if (normalized.Any(c => char.IsControl(c)))
            return Error.Validation(
                $"{fieldName.ToLower(System.Globalization.CultureInfo.CurrentCulture)}.invalid_format", 
                $"Адрес {fieldName} не должен содержать управляющих символов");

        return normalized;
    }
    public override string ToString() => $"{City}, {Street}, {House}, {Apartment}";
}