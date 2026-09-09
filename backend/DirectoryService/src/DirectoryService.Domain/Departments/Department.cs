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

    public Department Parent { get; private set; } = null!;
    public Name Name { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;

    public ValueObjects.Path? Path { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<DepartmentLocation> DepartmentLocations { get; private set; } = new List<DepartmentLocation>();

    public ICollection<DepartmentPosition> DepartmentPositions { get; private set; } = new List<DepartmentPosition>();

    public ICollection<Department> Children { get; private set; } = new List<Department>();

    public static Result<Department, DomainError> Create(string name, string slug, DepartmentId? parentId = null, ValueObjects.Path? parentPath = null)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        var pathResult = ValueObjects.Path.Create(parentPath, slugResult.Value);

        if (parentId == null)
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
            parentId,
            nameResult.Value,
            slugResult.Value,
            pathResult.Value,
            DateTime.UtcNow);
    }

    public Result<DepartmentId, DomainError> UpdateName(string name)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        Name = nameResult.Value;
        UpdatedAt = DateTime.UtcNow;

        return Id;
    }

    public Result<DepartmentId, DomainError> UpdateSlug(string slug)
    {
        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        Slug = slugResult.Value;
        UpdatedAt = DateTime.UtcNow;

        return Id;
    }
}