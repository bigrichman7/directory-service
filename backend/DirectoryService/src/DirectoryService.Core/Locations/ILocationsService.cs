using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Location;
using DirectoryService.Domain.Locations;
using ErrorOr;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);

    Task<Result<Location, Error>> Update(UpdateLocationDto locationDto, CancellationToken cancellationToken);
}
