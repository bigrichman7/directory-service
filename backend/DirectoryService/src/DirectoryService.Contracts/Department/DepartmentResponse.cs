using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Department;

public record DepartmentResponse(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Slug,
    string? Path,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
