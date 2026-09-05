using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Department;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Result<Guid, Error>> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken);
}
