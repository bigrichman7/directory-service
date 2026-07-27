using CSharpFunctionalExtensions;
using System;
using TemplateService.Domain.Common.Errors;

namespace TemplateService.Domain.Departments;
public sealed class DepartmentLocation
{
	private DepartmentLocation(Guid id, Guid departmentId, Guid locationId, bool isPrimary)
	{
        Id = id;
        DepartmentId = departmentId;
        LocationId = locationId;
        IsPrimary = isPrimary;
    }

    private DepartmentLocation() { }

	public Guid Id { get; private set; }
	public Guid DepartmentId { get; private set; }
    public Guid LocationId { get; private set; }

	public bool IsPrimary { get; private set; }

    public static Result<DepartmentLocation, DomainError> Create(Guid departmentId, Guid locationId, bool isPrimary = false)
    {
        if (departmentId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("DepartmentId не может быть пустым");

        if (locationId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("LocationId не может быть пустым");


        return new DepartmentLocation(Guid.CreateVersion7(), departmentId, locationId, isPrimary);
    }

}
