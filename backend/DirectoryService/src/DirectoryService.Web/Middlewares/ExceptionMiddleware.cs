using Shared;
using Shared.Exceptions;
using System.Text.Json;
using DirectoryService.Web.EndpointResults;

namespace DirectoryService.Web.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(
            exception,
            "Необработанное исключение при запросе {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        var errorResult = new ErrorResult(Error.Failure(code: "directory_service.error", "Непредвиденная ошибка сервера"));

        await errorResult.ExecuteAsync(context);
    }
}


public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this WebApplication app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}