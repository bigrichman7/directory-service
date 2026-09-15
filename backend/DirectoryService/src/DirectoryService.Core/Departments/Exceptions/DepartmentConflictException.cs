using Shared;
using Shared.Exceptions;

namespace DirectoryService.Core.Departments.Exceptions;

public class DepartmentConflictException(Error[] errors) : ConflictException(errors)
{
    public DepartmentConflictException(Error error)
        : this([error])
    {
    }
}
