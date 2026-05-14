using EnterpriseCMS.Core.Interfaces;

namespace EnterpriseCMS.Web.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    public TenantMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext ctx) => await _next(ctx);
}
