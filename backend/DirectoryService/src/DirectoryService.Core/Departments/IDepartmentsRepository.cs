using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using ErrorOr;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Guid> UpdateAsync(Department department, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetByNameAsync(Name name, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetBySlugAsync(Slug slug, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    Task<Result<Domain.ValueObjects.Path, Error>> GetPathByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    Task<Result<DepartmentLocation, Error>> AddDepartmentLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);

    Task<Result<DepartmentLocation, Error>> RemoveDepartmentLocationAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
}
