using EnterpriseCMS.Web.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace EnterpriseCMS.Web.HealthChecks;

public class MaintenanceModeHealthCheck : IHealthCheck
{
    private readonly MaintenanceModeOptions _opts;
    public MaintenanceModeHealthCheck(IOptions<MaintenanceModeOptions> opts) => _opts = opts.Value;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        var result = _opts.IsEnabled
            ? HealthCheckResult.Degraded("Site is in maintenance mode")
            : HealthCheckResult.Healthy("Site is operational");
        return Task.FromResult(result);
    }
}
