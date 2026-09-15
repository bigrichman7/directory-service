using Shared;
using Shared.Exceptions;

namespace DirectoryService.Core.Locations.Exceptions;

public class LocationBadRequestException(Error[] errors) : BadRequestException(errors)
{
    public LocationBadRequestException(Error error)
        : this([error])
    {
    }
}
