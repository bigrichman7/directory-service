using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Department;
using Shared;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Guid> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken);

    Task<DepartmentLocationResponse> AddLocation(Guid departmentId, Guid locationId, bool isPrimary, CancellationToken cancellationToken);

    Task<DepartmentResponse> Update(Guid departmentId, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken);

    Task<DepartmentLocationResponse> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}
