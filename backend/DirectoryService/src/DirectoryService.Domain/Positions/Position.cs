using CSharpFunctionalExtensions;
using System;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Positions;

public record PositionId(Guid Value);

public sealed class Position
{
    private Position(PositionId id, string name, DateTime createdAt)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public PositionId Id { get; private set; }

    public string Name { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public ICollection<DepartmentPosition> DepartmentPositions { get; private set; } = new List<DepartmentPosition>();

    public static Result<Position, DomainError> Create(string name)
    {
        var nameResult = ValueObjects.Name.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var position = new Position(
            new PositionId(Guid.CreateVersion7()),
            nameResult.Value,
            DateTime.UtcNow);

        return position;
    }
}
