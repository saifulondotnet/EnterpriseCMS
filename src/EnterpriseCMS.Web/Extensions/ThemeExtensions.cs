using Microsoft.Extensions.FileProviders;

namespace EnterpriseCMS.Web.Extensions;

public static class ThemeExtensions
{
    public static IServiceCollection AddThemeEngine(this IServiceCollection services, IConfiguration config)
    {
        var activeTheme = config["Cms:ActiveTheme"] ?? "CMS-Clean";
        services.Configure<ThemeOptions>(opts => opts.ActiveTheme = activeTheme);
        return services;
    }

    public static IApplicationBuilder UseThemeStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Serve theme static files from /wwwroot/themes/{theme}/ under /themes/
        var themesPath = Path.Combine(env.ContentRootPath, "Themes");
        if (Directory.Exists(themesPath))
        {
            foreach (var themeDir in Directory.GetDirectories(themesPath))
            {
                var wwwroot = Path.Combine(themeDir, "wwwroot");
                if (Directory.Exists(wwwroot))
                {
                    var themeName = Path.GetFileName(themeDir);
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(wwwroot),
                        RequestPath = $"/themes/{themeName}"
                    });
                }
            }
        }
        return app;
    }
}

public class ThemeOptions
{
    public string ActiveTheme { get; set; } = "CMS-Clean";
}
