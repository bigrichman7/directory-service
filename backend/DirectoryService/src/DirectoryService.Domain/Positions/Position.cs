using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Positions;

public record PositionId(Guid Value);

public sealed class Position
{
    private Position() { }
    private Position(PositionId id, Name name, DateTime createdAt)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public PositionId Id { get; private set; } = null!;

    public Name Name { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public ICollection<DepartmentPosition> DepartmentPositions { get; private set; } = [];

    public static Result<Position, Error> Create(string name)
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
