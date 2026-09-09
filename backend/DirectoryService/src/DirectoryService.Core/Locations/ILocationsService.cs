using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Location;
using DirectoryService.Domain.Locations;
using ErrorOr;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);

    Task<Result<LocationResponse, Error>> Update(Guid locationId, UpdateLocationDto locationDto, CancellationToken cancellationToken);
}
