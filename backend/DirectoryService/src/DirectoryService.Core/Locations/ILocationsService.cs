using DirectoryService.Contracts.Location;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);
}
