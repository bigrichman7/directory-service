namespace DirectoryService.Contracts.Department;

public record CreateDepartmentDto(string Name, string Slug, Guid ParentId);
