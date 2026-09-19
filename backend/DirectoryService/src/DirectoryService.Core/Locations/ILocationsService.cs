using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Location;
using Shared;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Result<Guid, Error>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);

    Task<Result<LocationResponse, Error>> Update(Guid locationId, UpdateLocationDto locationDto, CancellationToken cancellationToken);
}
