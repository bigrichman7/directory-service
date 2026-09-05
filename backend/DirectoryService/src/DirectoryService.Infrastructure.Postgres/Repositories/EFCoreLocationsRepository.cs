using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using ErrorOr;
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

    public async Task<Result<Guid, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Id == new LocationId(id), cancellationToken);

        if (location is null)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена", id);
            return Error.NotFound("directory.location.not_found", $"Локация с Id {id} не найдена");
        }

        return location.Id.Value;
    }

    public async Task<Result<List<Location>, Error>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idsList = ids.Distinct().ToList();
        if (idsList.Count == 0)
        {
            return new List<Location>();
        }

        var locationIds = idsList.Select(id => new LocationId(id)).ToList();

        var locations = await _dbContext.Locations
            .Where(x => locationIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return locations;
    }

    public async Task<Result<Guid, Error>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .FirstOrDefaultAsync(x => x.Name.Value == name, cancellationToken);

        if (location is null)
        {
            _logger.LogError("Локация с именем {Name} не найдена", name);
            return Error.NotFound("directory.location.not_found", $"Локация с именем {name} не найдена");
        }

        return location.Id.Value;
    }
}
