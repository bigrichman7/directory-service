using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Shared;

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

    public DepartmentLocationId Id { get; private set; }
    public DepartmentId DepartmentId { get; private set; }
    public Department Department {  get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public LocationId LocationId { get; private set; }

	public bool IsPrimary { get; private set; }

    public static Result<DepartmentLocation, Error> Create(DepartmentId departmentId, LocationId locationId, bool isPrimary = false)
    {
        if (departmentId.Value == Guid.Empty)
            return Error.Validation("departmentId.is.empty", "DepartmentId не может быть пустым", "DepartmentId");

        if (locationId.Value == Guid.Empty)
            return Error.Validation("locationId.is.empty", "LocationId не может быть пустым", "LocationId");


        return new DepartmentLocation(new DepartmentLocationId(Guid.CreateVersion7()), departmentId, locationId, isPrimary);
    }

}
