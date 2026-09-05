using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using ErrorOr;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<List<Location>, Error>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
