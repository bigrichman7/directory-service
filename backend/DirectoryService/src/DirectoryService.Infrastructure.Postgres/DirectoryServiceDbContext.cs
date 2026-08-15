using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

public class DirectoryServiceDbContext : DbContext
{
    public DirectoryServiceDbContext(DbContextOptions<DirectoryServiceDbContext> options)
        : base(options)
    {
         
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);

    }
}