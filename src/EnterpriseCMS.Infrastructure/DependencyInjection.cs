using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Interfaces;
using EnterpriseCMS.Infrastructure.BackgroundJobs;
using EnterpriseCMS.Infrastructure.Data;
using EnterpriseCMS.Infrastructure.Identity;
using EnterpriseCMS.Infrastructure.Repositories;
using EnterpriseCMS.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace EnterpriseCMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // EF Core
        services.AddDbContext<CmsDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)
                          .EnableRetryOnFailure()));

        // Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(opts =>
        {
            opts.Password.RequiredLength = 8;
            opts.Password.RequireDigit = true;
            opts.Password.RequireUppercase = true;
            opts.Lockout.MaxFailedAccessAttempts = 5;
            opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            opts.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<CmsDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IPasswordHasher<ApplicationUser>, Argon2PasswordHasher>();

        // Core services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<ISlugService, SlugService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddScoped<ImageProcessingService>();
        services.AddScoped<ImageProcessingJob>();
        services.AddScoped<BackupJob>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISchemaOrgService, SchemaOrgService>();
        services.AddScoped<ScheduledContentJob>();
        services.AddScoped<DbSeeder>();
        services.AddScoped<DbInitializer>();

        // Redis cache
        var redisConn = config.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConn))
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
        }

        // Hangfire
        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddInfrastructureSerilog(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(lb => lb.AddSerilog(dispose: true));
        return services;
    }
}
