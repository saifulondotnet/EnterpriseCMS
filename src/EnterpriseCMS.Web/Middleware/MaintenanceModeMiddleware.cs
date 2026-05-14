using Microsoft.Extensions.Options;

namespace EnterpriseCMS.Web.Middleware;

public class MaintenanceModeOptions
{
    public bool IsEnabled { get; set; }
    public string? AllowedIp { get; set; }
    public string Message { get; set; } = "The site is currently undergoing maintenance. Please check back shortly.";
}

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MaintenanceModeOptions _opts;

    public MaintenanceModeMiddleware(RequestDelegate next, IOptions<MaintenanceModeOptions> opts)
    { _next = next; _opts = opts.Value; }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!_opts.IsEnabled) { await _next(ctx); return; }

        var remoteIp = ctx.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrWhiteSpace(_opts.AllowedIp) && remoteIp == _opts.AllowedIp)
        { await _next(ctx); return; }

        // Admin paths bypass
        if (ctx.Request.Path.StartsWithSegments("/admin") || ctx.Request.Path.StartsWithSegments("/health"))
        { await _next(ctx); return; }

        ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        ctx.Response.Headers["Retry-After"] = "3600";
        ctx.Response.ContentType = "text/html";
        await ctx.Response.WriteAsync($"<html><body><h1>Maintenance</h1><p>{_opts.Message}</p></body></html>");
    }
}

public static class MaintenanceModeExtensions
{
    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder app)
        => app.UseMiddleware<MaintenanceModeMiddleware>();
}
