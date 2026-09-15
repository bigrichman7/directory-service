using Shared;
using Shared.Exceptions;

namespace DirectoryService.Core.Locations.Exceptions;

public class LocationConflictException(Error[] errors) : ConflictException(errors)
{
    public LocationConflictException(Error error)
        : this([error])
    {
    }
}
