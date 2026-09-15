using Shared;
using Shared.Exceptions;

namespace DirectoryService.Core.Departments.Exceptions;

public class DepartmentNotFoundException(Error[] errors) : NotFoundException(errors)
{
    public DepartmentNotFoundException(Error error)
        : this([error])
    {
    }
}
