using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Department;

public record DepartmentLocationResponse(
    Guid DepartmnetLocationId,
    Guid DepartmentId,
    Guid LocationId,
    bool IsPrimary
);
