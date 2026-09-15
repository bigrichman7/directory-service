using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Guid> UpdateAsync(Department department, CancellationToken cancellationToken);

    Task<Guid?> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken);

    Task<Department?> GetByNameAsync(Name name, CancellationToken cancellationToken);

    Task<Department?> GetBySlugAsync(Slug slug, CancellationToken cancellationToken);

    Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    Task<DepartmentLocation?> AddDepartmentLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);

    Task<DepartmentLocation?> RemoveDepartmentLocationAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
}
