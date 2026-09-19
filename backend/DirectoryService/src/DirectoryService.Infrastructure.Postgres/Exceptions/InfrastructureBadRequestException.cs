using Shared.Exceptions;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Exceptions;

public class InfrastructureBadRequestException : BadRequestException
{
    public InfrastructureBadRequestException(Error error)
        : base([error])
    {
    }

    public InfrastructureBadRequestException(Error[] errors) : base(errors)
    {
    }
}
