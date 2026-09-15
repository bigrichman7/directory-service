using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web;
using DirectoryService.Web.Middlewares;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies();

var connectionString = builder.Configuration.GetConnectionString("DirectoryServiceDb");

builder.Services.AddDbContext<DirectoryServiceDbContext>(options => options.UseNpgsql(connectionString));

WebApplication app = builder.Build();

app.UseExceptionMiddleware();

// Minimal API endpoints
app.MapGet("/", () => "DirectoryService is running!");

app.MapHealthChecks("/api/health");

// MVC контроллеры
app.MapControllers();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();              // /openapi/v1.json
    app.MapScalarApiReference();   // /scalar/v1
}

await app.RunAsync();