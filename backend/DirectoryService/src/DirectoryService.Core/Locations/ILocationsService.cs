using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Location;
using Shared;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);

    Task<LocationResponse> Update(Guid locationId, UpdateLocationDto locationDto, CancellationToken cancellationToken);
}
