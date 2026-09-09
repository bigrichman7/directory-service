namespace DirectoryService.Contracts.Department;

public record UpdateDepartmentDto(Guid Id, string? Name, string? Slug, Guid? ParentId);
