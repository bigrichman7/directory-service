using Shared.Exceptions;
using Shared;

namespace DirectoryService.Core.Locations.Exceptions;

public class LocationNotFoundException(Error[] errors) : NotFoundException(errors)
{
    public LocationNotFoundException(Error error)
        : this ([error])
    {
    }
}
