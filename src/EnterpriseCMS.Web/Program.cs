using EnterpriseCMS.Application;
using EnterpriseCMS.Core.Interfaces;
using EnterpriseCMS.Infrastructure;
using EnterpriseCMS.Infrastructure.BackgroundJobs;
using EnterpriseCMS.Infrastructure.Data;
using EnterpriseCMS.Web.Extensions;
using EnterpriseCMS.Web.HealthChecks;
using EnterpriseCMS.Web.Middleware;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddThemeEngine(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Authentication - Cookie (for Admin MVC) + JWT (for API)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "MultiAuth";
    options.DefaultChallengeScheme = "MultiAuth";
})
.AddCookie("AdminCookies", opts =>
{
    opts.LoginPath = "/Admin/Account/Login";
    opts.AccessDeniedPath = "/Admin/Account/AccessDenied";
    opts.ExpireTimeSpan = TimeSpan.FromHours(8);
    opts.SlidingExpiration = true;
    opts.Cookie.HttpOnly = true;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    opts.Cookie.SameSite = SameSiteMode.Strict;
})
.AddJwtBearer("Bearer", opts =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretDevKey1234567890123456";
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EnterpriseCMS",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EnterpriseCMS",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
})
.AddPolicyScheme("MultiAuth", "MultiAuth", opts =>
{
    opts.ForwardDefaultSelector = ctx =>
    {
        var auth = ctx.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return "Bearer";
        return "AdminCookies";
    };
});

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("Admin", p => p.RequireRole(
        EnterpriseCMS.Core.Interfaces.RoleNames.SuperAdmin,
        EnterpriseCMS.Core.Interfaces.RoleNames.Administrator,
        EnterpriseCMS.Core.Interfaces.RoleNames.Editor,
        EnterpriseCMS.Core.Interfaces.RoleNames.Author));
});

// Rate limiting
builder.Services.AddRateLimiter(opts =>
{
    opts.AddFixedWindowLimiter("fixed", rl =>
    {
        rl.PermitLimit = 20;
        rl.Window = TimeSpan.FromMinutes(1);
        rl.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        rl.QueueLimit = 5;
    });
});

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<MaintenanceModeHealthCheck>("maintenance");

// Maintenance mode
builder.Services.Configure<MaintenanceModeOptions>(
    builder.Configuration.GetSection("Maintenance"));

// Memory cache for redirect middleware
builder.Services.AddMemoryCache();

// Anti-forgery
builder.Services.AddAntiforgery(opts =>
{
    opts.Cookie.HttpOnly = true;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    opts.Cookie.SameSite = SameSiteMode.Strict;
    opts.HeaderName = "X-CSRF-TOKEN";
});

// MVC + API
builder.Services.AddControllersWithViews(opts =>
{
    opts.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EnterpriseCMS API", Version = "v1" });
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSecurityHeaders();
app.UseMaintenanceMode();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseThemeStaticFiles(app.Environment);
app.UseRedirectMiddleware();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Hangfire dashboard (admin only)
app.UseHangfireDashboard("/admin/jobs", new DashboardOptions
{
    Authorization = new[] { new EnterpriseCMS.Web.Extensions.HangfireAuthFilter() }
});

// Register recurring Hangfire jobs
RecurringJob.AddOrUpdate<ScheduledContentJob>(
    "publish-scheduled-content",
    job => job.ExecuteAsync(),
    Cron.Minutely);

// Health check endpoint
app.MapHealthChecks("/health");

// Route for Admin area
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Slug route for CMS pages
app.MapControllerRoute(
    name: "cmspage",
    pattern: "{slug}",
    defaults: new { controller = "Home", action = "Page" });

// Apply migrations and seed reference/demo data
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<EnterpriseCMS.Infrastructure.Data.DbInitializer>();
    await initializer.InitialiseAsync();
}

app.Run();
