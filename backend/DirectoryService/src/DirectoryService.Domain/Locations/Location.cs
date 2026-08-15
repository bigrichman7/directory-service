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
	private Location(LocationId id, string name, string address, DateTime createdAt)
	{
        Id = id;
        Name = name;
        Address = address;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

	public LocationId Id { get; private set; }

	public string Name { get; private set; }

    public string Address { get; private set; }

    public DateTime CreatedAt { get; private set; }

	public DateTime UpdatedAt { get; private set; }

    public ICollection<DepartmentLocation> DepartmentLocations { get; private set; } = new List<DepartmentLocation>();

    public static Result<Location, DomainError> Create(string name, string address)
    {
        var nameResult = ValueObjects.Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = ValueObjects.Address.Create(address);
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
