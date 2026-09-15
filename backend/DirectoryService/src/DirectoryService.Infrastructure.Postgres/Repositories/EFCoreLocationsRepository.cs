using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class EFCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EFCoreLocationsRepository> _logger;

    public EFCoreLocationsRepository(DirectoryServiceDbContext dbContext, ILogger<EFCoreLocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Новая локация с Id {LocationId} добавлена", location.Id.Value);

        return location.Id.Value;
    }

    public async Task<Location> UpdateAsync(Location location, CancellationToken cancellationToken)
    {

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Локация с Id {LocationId} обновлена", location.Id.Value);

        return location;
    }

    public async Task<Location?> GetByIdAsync(LocationId id, CancellationToken cancellationToken)
    {
        return await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idsList = ids.Distinct().ToList();
        if (idsList.Count == 0)
        {
            return [];
        }

        var locationIds = idsList.ConvertAll(id => new LocationId(id));

        return await _dbContext.Locations
            .Where(x => locationIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Location?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Name.Value == name, cancellationToken);
    }
}
