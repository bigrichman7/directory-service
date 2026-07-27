using CSharpFunctionalExtensions;
using TemplateService.Domain.Common.Errors;
using TemplateService.Domain.ValueObjects;

namespace TemplateService.Domain.Departments;

public sealed class Department
{
    private Department() { }

    private Department(Guid id, Guid? parentId, Name name, Slug slug, ValueObjects.Path path, DateTime createdAt)
    {
        Id = id;
        ParentId = parentId;
        Name = name;
        Slug = slug;
        Path = path;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid? ParentId { get; private set; }
    public Name? Name { get; private set; }
    public Slug? Slug { get; private set; }

    public ValueObjects.Path? Path { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<Department, DomainError> Create(string name, string slug, Guid? parentId = null)
    {
        var nameResult = Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        if (parentId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("ParentId не может быть Guid.Empty");

        var pathResult = ValueObjects.Path.Create(parentId, slugResult.Value);

        var department = new Department(
            Guid.CreateVersion7(),
            parentId,
            nameResult.Value,
            slugResult.Value,
            pathResult.Value,
            DateTime.UtcNow);

        return department;
    }

}
