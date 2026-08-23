using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Department;

public record UpdateDepartmentDto(string Name, string Slug, Guid ParentId);
