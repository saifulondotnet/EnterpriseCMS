using EnterpriseCMS.Core.Exceptions;

namespace EnterpriseCMS.Web.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(string.Empty);
            }
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 404;
                context.Response.Redirect("/Home/Error404");
            }
        }
        catch (ForbiddenException ex)
        {
            _logger.LogWarning(ex, "Access forbidden: {Message}", ex.Message);
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 403;
                context.Response.Redirect("/Home/Error500");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                context.Response.Redirect("/Home/Error500");
            }
        }
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}
