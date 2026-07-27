using CSharpFunctionalExtensions;
using System;
using TemplateService.Domain.Common.Errors;

namespace TemplateService.Domain.Positions;

public sealed class DepartmentPosition
{
    private DepartmentPosition(Guid id, Guid departmentId, Guid positionId)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    private DepartmentPosition() { }

    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }

    public static Result<DepartmentPosition, DomainError> Create(Guid id, Guid departmentId, Guid positionId)
    {
        if (departmentId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("DepartmentId не может быть пустым");

        if (positionId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid("PositionId не может быть пустым");

        return new DepartmentPosition(Guid.CreateVersion7(), departmentId, positionId);
    }
}
