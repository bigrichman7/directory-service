using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Departments;

public record DepartmentId(Guid Value);

public sealed class Department
{
    private Department() { }

    private Department(DepartmentId id, DepartmentId? parentId, Name name, Slug slug, ValueObjects.Path path, DateTime createdAt)
    {
        Id = id;
        ParentId = parentId;
        Name = name;
        Slug = slug;
        Path = path;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public DepartmentId Id { get; } = null!;
    public DepartmentId? ParentId { get; private set; }

    public Department Parent {  get; private set; } = null!;
    public Name? Name { get; private set; }
    public Slug? Slug { get; private set; }

    public ValueObjects.Path? Path { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<DepartmentLocation> DepartmentLocations { get; private set; } = new List<DepartmentLocation>();

    public ICollection<DepartmentPosition> DepartmentPositions { get; private set; } = new List<DepartmentPosition>();

    public ICollection<Department> Children { get; private set; } = new List<Department>();

    public static Result<Department, DomainError> Create(string name, string slug, Department? parentDepartment = null)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        var pathResult = ValueObjects.Path.Create(parentDepartment, slugResult.Value);

        if (parentDepartment == null)
        {
            return new Department(
            new DepartmentId(Guid.CreateVersion7()),
            null,
            nameResult.Value,
            slugResult.Value,
            pathResult.Value,
            DateTime.UtcNow);
        }

        return new Department(
            new DepartmentId(Guid.CreateVersion7()),
            parentDepartment.Id,
            nameResult.Value,
            slugResult.Value,
            pathResult.Value,
            DateTime.UtcNow);
    }

}
