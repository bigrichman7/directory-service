using Shared;
using Shared.Exceptions;

namespace DirectoryService.Core.Departments.Exceptions;

public class DepartmentBadRequestException(Error[] errors) : BadRequestException(errors)
{
    public DepartmentBadRequestException(Error error)
        : this([error])
    {
    }
}
