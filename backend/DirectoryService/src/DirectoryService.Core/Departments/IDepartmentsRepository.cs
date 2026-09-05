using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.ValueObjects;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);
    
    Task<Result<Guid, Error>> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken);
   
    Task<Result<Guid, Error>> GetByNameAsync(Name name, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetBySlugAsync(Slug slug, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    Task<Result<Domain.ValueObjects.Path, Error>> GetPathByIdAsync(DepartmentId id, CancellationToken cancellationToken);

}
