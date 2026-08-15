using CSharpFunctionalExtensions;
using System;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Domain.Departments;

public record DepartmentLocationId(Guid Value);
public sealed class DepartmentLocation
{
	private DepartmentLocation(DepartmentLocationId id, DepartmentId departmentId, LocationId locationId, bool isPrimary)
	{
        Id = id;
        DepartmentId = departmentId;
        LocationId = locationId;
        IsPrimary = isPrimary;
    }

    private DepartmentLocation() { }

    public DepartmentLocationId Id { get; private set; } = null!;

    public DepartmentId DepartmentId { get; private set; } = null!;
    public Department Department {  get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public LocationId LocationId { get; private set; } = null!;

	public bool IsPrimary { get; private set; }

    public static Result<DepartmentLocation, DomainError> Create(DepartmentId departmentId, LocationId locationId, bool isPrimary = false)
    {
        if (departmentId.Value == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("DepartmentId не может быть пустым");

        if (locationId.Value == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("LocationId не может быть пустым");


        return new DepartmentLocation(new DepartmentLocationId(Guid.CreateVersion7()), departmentId, locationId, isPrimary);
    }

}
