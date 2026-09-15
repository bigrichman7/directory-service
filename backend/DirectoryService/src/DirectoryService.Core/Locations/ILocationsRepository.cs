using DirectoryService.Domain.Locations;


namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Location> UpdateAsync(Location location, CancellationToken cancellationToken);

    Task<Location?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<Location?> GetByIdAsync(LocationId id, CancellationToken cancellationToken);

    Task<IEnumerable<Location>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
