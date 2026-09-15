using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Shared;

namespace DirectoryService.Domain.Positions;

public record DepartmentPositionId(Guid Value);

public sealed class DepartmentPosition
{
    private DepartmentPosition(DepartmentPositionId id, DepartmentId departmentId, PositionId positionId)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentPositionId Id { get; private set; }
    public DepartmentId DepartmentId { get; private set; }
    public PositionId PositionId { get; private set; }

    public Position Position { get; private set; } = null!;

    public Department Department { get; private set; } = null!;

    public static Result<DepartmentPosition, Failure> Create(DepartmentId departmentId, PositionId positionId)
    {
        if (departmentId.Value == Guid.Empty)
            return Error.Validation("DepartmentId", "DepartmentId не может быть пустым").ToFailure();

        if (positionId.Value == Guid.Empty)
            return Error.Validation("PositionId", "PositionId не может быть пустым").ToFailure();

        return new DepartmentPosition(new DepartmentPositionId(Guid.CreateVersion7()), departmentId, positionId);
    }
}
