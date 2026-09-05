using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

        //services.AddScoped<ILocationsRepository, NpgSqlLocationsRepository>();
        services.AddScoped<ILocationsRepository, EFCoreLocationsRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();

        return services;
    }
}
