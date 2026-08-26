using DirectoryService.Contracts.Location;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);
}
