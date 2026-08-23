using CSharpFunctionalExtensions;
using System;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Departments;

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

    public static Result<DepartmentPosition, DomainError> Create(DepartmentId departmentId, PositionId positionId)
    {
        if (departmentId.Value == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("DepartmentId не может быть пустым");

        if (positionId.Value == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("PositionId не может быть пустым");

        return new DepartmentPosition(new DepartmentPositionId(Guid.CreateVersion7()), departmentId, positionId);
    }
}
