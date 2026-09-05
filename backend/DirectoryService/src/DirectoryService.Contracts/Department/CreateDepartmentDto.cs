namespace DirectoryService.Contracts.Department;

public record CreateDepartmentDto(string Name, string Slug, IEnumerable<Guid>? LocationIds = null, Guid? ParentId = null);
