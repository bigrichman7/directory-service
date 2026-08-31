using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;

namespace DirectoryService.Web;

public static class WebDependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        return services
            .AddWebDependencies()
            .AddApplication()
            .AddInfrastructure();
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddHealthChecks();

        return services;
    }
}
