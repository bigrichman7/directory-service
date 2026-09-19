using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Shared;


namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Location> UpdateAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByIdAsync(LocationId id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Location>, Error>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
