using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);
    
    Task<Result<Guid, Error>> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken);
   
    Task<Result<Guid, Error>> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Domain.ValueObjects.Path, Error>> GetPathByIdAsync(Guid id, CancellationToken cancellationToken);

}
