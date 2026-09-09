using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Department;

public record DepartmentLocationResponse(
    Guid DepartmentLocationId,
    Guid DepartmentId,
    Guid LocationId,
    bool IsPrimary
);
