using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class EFCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public EFCoreLocationsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return location.Id.Value;
    }

    public async Task<Location> UpdateAsync(Location location, CancellationToken cancellationToken)
    {

        await _dbContext.SaveChangesAsync(cancellationToken);

        return location;
    }

    public async Task<Result<Location, Error>> GetByIdAsync(LocationId id, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (location == null)
        {
            return Error.NotFound("location.not_found", $"Локация с Id {id} не найдена.");
        }

        return location;
    }

    public async Task<Result<IEnumerable<Location>, Error>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idsList = ids.Distinct().ToList();
        if (idsList.Count == 0)
        {
            return Error.NotFound("locations.not_found", $"Локации с указанными {ids} не найдены.");
        }

        var locationIds = idsList.ConvertAll(id => new LocationId(id));

        return await _dbContext.Locations
            .Where(x => locationIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<Location, Error>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Name.Value == name, cancellationToken);

        if (location == null)
        {
            return Error.NotFound("location.not_found", $"Локация с именем {name} не найдена.");
        }

        return location;
    }
}
