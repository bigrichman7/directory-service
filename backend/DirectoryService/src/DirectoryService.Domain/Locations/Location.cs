using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.ValueObjects;
using ErrorOr;
using System;

namespace DirectoryService.Domain.Locations;

public record LocationId(Guid Value);

public sealed class Location
{
    private Location() { }
	private Location(LocationId id, Name name, Address address, DateTime createdAt)
	{
        Id = id;
        Name = name;
        Address = address;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public LocationId Id { get; private set; } = null!;

	public Name Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

	public DateTime UpdatedAt { get; private set; }

    public ICollection<DepartmentLocation> DepartmentLocations { get; private set; } = new List<DepartmentLocation>();

    public static Result<Location, DomainError> Create(string name, string city, string street, string house, string apartment)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = Address.Create(city, street, house, apartment);
        if (addressResult.IsFailure)
            return addressResult.Error;


        var location = new Location(
            new LocationId(Guid.CreateVersion7()),
            nameResult.Value,
            addressResult.Value,
            DateTime.UtcNow);

        return location;
    }
}
