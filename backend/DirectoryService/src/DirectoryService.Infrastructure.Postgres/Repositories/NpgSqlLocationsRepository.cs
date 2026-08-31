using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Infrastructure.Postgres.Database;
using ErrorOr;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class NpgSqlLocationsRepository : ILocationsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<NpgSqlLocationsRepository> _logger;

    public NpgSqlLocationsRepository(IDbConnectionFactory connectionFactory, ILogger<NpgSqlLocationsRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        const string locationInsertSql = """
                                         INSERT INTO locations (id, name, created_at, updated_at, city, street, house, apartment)
                                         VALUES (@Id, @Name, @Created_At, @Updated_At, @City, @Street, @House, @Apartment)
                                         """;
        var locationInsertParams = new
        {
            Id = location.Id.Value,
            Name = location.Name.Value,
            Created_At = location.CreatedAt,
            Updated_At = location.UpdatedAt,
            City = location.Address.City,
            Street = location.Address.Street,
            House = location.Address.House,
            Apartment = location.Address.Apartment
        };

        await connection.ExecuteAsync(locationInsertSql, locationInsertParams);

        _logger.LogInformation("New location with id {LocationId} was added", location.Id.Value);

        return location.Id.Value;
    }

    public async Task<Result<Guid, Error>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        const string locationGetNameSql = """
                                         SELECT * FROM locations WHERE name = @name LIMIT 1                                     
                                         """;

        var locationId = await connection.QueryFirstOrDefaultAsync<Guid?>(locationGetNameSql, new { name });

        if (locationId == null)
        {
            _logger.LogError("Location with Name: {Name} not found", name);
            return Error.NotFound("location.not.found", $"Location with name '{name}' not found");
        }

        return locationId.Value;
    }
}
