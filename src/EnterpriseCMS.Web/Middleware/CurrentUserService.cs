using System.Security.Claims;
using EnterpriseCMS.Core.Interfaces;

namespace EnterpriseCMS.Web.Middleware;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserService(IHttpContextAccessor httpContext) => _httpContext = httpContext;

    public Guid? UserId
    {
        get
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                     ?? _httpContext.HttpContext?.User?.FindFirst("sub");
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst("tenantId");
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
        }
    }

    public string? UserName => _httpContext.HttpContext?.User?.Identity?.Name;

    public bool IsAuthenticated => _httpContext.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => _httpContext.HttpContext?.User?.IsInRole(role) ?? false;
}
