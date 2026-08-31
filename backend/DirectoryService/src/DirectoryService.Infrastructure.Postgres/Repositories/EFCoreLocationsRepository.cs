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

        _logger.LogInformation("New location with id {LocationId} was added", location.Id.Value);

        return location.Id.Value;
    }

    public async Task<Result<Guid, Error>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .Where(x => x.Name.Value == name)
            .FirstOrDefaultAsync(cancellationToken);

        if (location is null)
        {
            _logger.LogError("Location with Name: {Name} not found", name);
            return Error.NotFound("directory.location.not_found", $"Локация с именем {name} не найдена");
        }

        return location.Id.Value;
    }
}
