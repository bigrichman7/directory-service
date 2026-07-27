using CSharpFunctionalExtensions;
using System;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Domain.Positions;

public sealed class DepartmentPosition
{
    private DepartmentPosition(Guid id, Guid departmentId, Guid positionId)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    private DepartmentPosition() { }

    public Guid Id { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid PositionId { get; private set; }

    public static Result<DepartmentPosition, DomainError> Create(Guid id, Guid departmentId, Guid positionId)
    {
        if (departmentId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("DepartmentId не может быть пустым");

        if (positionId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("PositionId не может быть пустым");

        return new DepartmentPosition(Guid.CreateVersion7(), departmentId, positionId);
    }
}
