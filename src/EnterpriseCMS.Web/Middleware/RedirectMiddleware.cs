using EnterpriseCMS.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace EnterpriseCMS.Web.Middleware;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;

    public RedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IMemoryCache cache, IServiceProvider serviceProvider)
    {
        var path = context.Request.Path.Value ?? "/";

        var redirects = await cache.GetOrCreateAsync("cms_redirects", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            using var scope = serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            return await uow.Redirects.GetAllAsync();
        });

        if (redirects is not null)
        {
            foreach (var redirect in redirects)
            {
                bool isMatch;
                if (redirect.IsRegex)
                {
                    isMatch = Regex.IsMatch(path, redirect.FromSlug, RegexOptions.IgnoreCase);
                }
                else
                {
                    isMatch = string.Equals(path.TrimEnd('/'),
                        redirect.FromSlug.TrimEnd('/'),
                        StringComparison.OrdinalIgnoreCase);
                }

                if (isMatch)
                {
                    var toUrl = redirect.IsRegex
                        ? Regex.Replace(path, redirect.FromSlug, redirect.ToSlug, RegexOptions.IgnoreCase)
                        : redirect.ToSlug;

                    context.Response.StatusCode = redirect.StatusCode;
                    context.Response.Headers.Location = toUrl;
                    return;
                }
            }
        }

        await _next(context);
    }
}

public static class RedirectMiddlewareExtensions
{
    public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<RedirectMiddleware>();
}
