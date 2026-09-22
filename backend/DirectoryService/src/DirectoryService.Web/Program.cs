using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web;
using DirectoryService.Web.Middlewares;
using DirectoryService.Web.EndpointResults;
using Shared;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.OpenApi;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies();

var connectionString = builder.Configuration.GetConnectionString("DirectoryServiceDb");

builder.Services.AddDbContext<DirectoryServiceDbContext>(options => options.UseNpgsql(connectionString));


builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer((schema, context, _) =>
    {
        if (context.JsonTypeInfo.Type == typeof(Envelope<Error>)
        && schema.Properties != null
        && schema.Properties.ContainsKey("error"))
        {
            schema.Properties["error"] = new OpenApiSchemaReference("Error", null);
        }

        return Task.CompletedTask;
    });
});


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
    app.MapScalarApiReference(options =>
    {
        options.Title = "Directory Service API";
        options.Theme = ScalarTheme.Kepler;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });   // /scalar/v1
}

await app.RunAsync();