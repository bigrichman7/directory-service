using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Domain.ValueObjects;

public sealed record Address
{
    public const int MIN_LENGTH = 3;
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

    public static Result<Address, DomainError> Create(string city, string street, string house, string apartment)
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

    private static Result<string, DomainError> ValidAttribute(string attribute, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(attribute))
        {
            return GeneralErrors.ValueIsRequired($"Не задан параметр {fieldName}");
        }

        var normalized = attribute.Trim();

        if (normalized.Length < MIN_LENGTH)
            return GeneralErrors.ValueIsInvalid($"Адрес локации должен содержать минимум {MIN_LENGTH} символа");

        if (normalized.Length > MAX_LENGTH)
            return GeneralErrors.ValueIsInvalid($"Адрес локации не должен превышать {MAX_LENGTH} символов");

        if (normalized.Any(c => char.IsControl(c)))
            return GeneralErrors.ValueIsInvalid("Адрес локации не должен содержать управляющих символов");

        return normalized;
    }
    public override string ToString() => $"{City}, {Street}, {House}, {Apartment}";
}