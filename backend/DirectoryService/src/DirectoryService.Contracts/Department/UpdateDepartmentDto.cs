namespace DirectoryService.Contracts.Department;

public record UpdateDepartmentDto(string? Name, string? Slug, Guid? ParentId);
