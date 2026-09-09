using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments;
using ErrorOr;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Result<Guid, Error>> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken);

    Task<Result<DepartmentLocationResponse, Error>> AddLocation(Guid departmentId, Guid locationId, bool isPrimary, CancellationToken cancellationToken);

    Task<Result<DepartmentResponse, Error>> Update(UpdateDepartmentDto departmentDto, CancellationToken cancellationToken);

    Task<Result<DepartmentLocationResponse, Error>> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}
