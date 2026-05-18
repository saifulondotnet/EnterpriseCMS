using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EnterpriseCMS.Infrastructure.Data;

/// <summary>
/// Seeds essential reference data and development/smoke-test data.
/// Every method is idempotent — safe to call on every startup.
/// </summary>
public sealed class DbSeeder
{
    private readonly CmsDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(
        CmsDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger<DbSeeder> logger)
    {
        _context       = context;
        _userManager   = userManager;
        _roleManager   = roleManager;
        _configuration = configuration;
        _logger        = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync(cancellationToken);
        var adminId = await SeedUsersAsync(cancellationToken);
        await SeedSettingsAsync(cancellationToken);
        var categoryIds = await SeedCategoriesAsync(cancellationToken);
        var tagIds = await SeedTagsAsync(cancellationToken);
        await SeedPagesAsync(adminId, cancellationToken);
        await SeedPostsAsync(adminId, categoryIds, tagIds, cancellationToken);
        await SeedNavigationMenuAsync(cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Roles
    // -------------------------------------------------------------------------
    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        string[] roles =
        [
            RoleNames.SuperAdmin,
            RoleNames.Administrator,
            RoleNames.Editor,
            RoleNames.Author,
            RoleNames.Subscriber,
        ];

        foreach (var role in roles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await _roleManager.RoleExistsAsync(role))
                continue;

            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = role });
            if (result.Succeeded)
                _logger.LogInformation("Role '{Role}' created.", role);
            else
                _logger.LogError("Failed to create role '{Role}': {Errors}", role,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // -------------------------------------------------------------------------
    // Users — returns the SuperAdmin's Guid for use as AuthorId in content seeds
    // -------------------------------------------------------------------------
    private async Task<Guid> SeedUsersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var section   = _configuration.GetSection("Seed:SuperAdmin");
        var email     = section["Email"]     ?? "admin@enterprisecms.local";
        var password  = section["Password"]  ?? "Admin@123456";
        var firstName = section["FirstName"] ?? "Super";
        var lastName  = section["LastName"]  ?? "Admin";

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            _logger.LogInformation("SuperAdmin '{Email}' already exists.", email);
            return existing.Id;
        }

        var user = new ApplicationUser
        {
            UserName       = email,
            Email          = email,
            FirstName      = firstName,
            LastName       = lastName,
            DisplayName    = $"{firstName} {lastName}",
            IsActive       = true,
            EmailConfirmed = true,
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            _logger.LogError("Failed to create SuperAdmin: {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return Guid.Empty;
        }

        await _userManager.AddToRoleAsync(user, RoleNames.SuperAdmin);
        _logger.LogInformation("SuperAdmin '{Email}' seeded.", email);
        return user.Id;
    }

    // -------------------------------------------------------------------------
    // Settings
    // -------------------------------------------------------------------------
    private async Task SeedSettingsAsync(CancellationToken cancellationToken)
    {
        if (await _context.Settings.AnyAsync(cancellationToken: cancellationToken))
            return;

        Setting[] defaults =
        [
            new() { SettingKey = "site:name",           SettingValue = "EnterpriseCMS",            Group = "General", IsSystem = true,  Description = "Public name of the site" },
            new() { SettingKey = "site:tagline",         SettingValue = "A powerful CMS platform",  Group = "General", IsSystem = false, Description = "Site tagline" },
            new() { SettingKey = "site:description",     SettingValue = "Enterprise-grade CMS built on .NET 9", Group = "SEO",     IsSystem = false, Description = "Default meta description" },
            new() { SettingKey = "site:logo",            SettingValue = "",                          Group = "General", IsSystem = false, Description = "Logo URL" },
            new() { SettingKey = "site:favicon",         SettingValue = "",                          Group = "General", IsSystem = false, Description = "Favicon URL" },
            new() { SettingKey = "site:locale",          SettingValue = "en-US",                     Group = "General", IsSystem = true,  Description = "Default locale" },
            new() { SettingKey = "site:timezone",        SettingValue = "UTC",                       Group = "General", IsSystem = true,  Description = "Default timezone" },
            new() { SettingKey = "email:fromName",       SettingValue = "EnterpriseCMS",             Group = "Email",   IsSystem = true,  Description = "Sender display name" },
            new() { SettingKey = "email:fromAddress",    SettingValue = "",                          Group = "Email",   IsSystem = true,  Description = "Sender email address" },
            new() { SettingKey = "maintenance:enabled",  SettingValue = "false",                     Group = "System",  IsSystem = true,  Description = "Enable maintenance mode" },
        ];

        await _context.Settings.AddRangeAsync(defaults, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} default settings seeded.", defaults.Length);
    }

    // -------------------------------------------------------------------------
    // Categories
    // -------------------------------------------------------------------------
    private async Task<Dictionary<string, Guid>> SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Categories.IgnoreQueryFilters().AnyAsync(cancellationToken: cancellationToken))
        {
            return await _context.Categories.IgnoreQueryFilters()
                .ToDictionaryAsync(c => c.Slug, c => c.Id, cancellationToken);
        }

        Category[] categories =
        [
            new() { Name = "News",        Slug = "news",        SortOrder = 1 },
            new() { Name = "Tutorials",   Slug = "tutorials",   SortOrder = 2 },
            new() { Name = "Technology",  Slug = "technology",  SortOrder = 3 },
            new() { Name = "General",     Slug = "general",     SortOrder = 4 },
        ];

        await _context.Categories.AddRangeAsync(categories, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} categories seeded.", categories.Length);

        return categories.ToDictionary(c => c.Slug, c => c.Id);
    }

    // -------------------------------------------------------------------------
    // Tags
    // -------------------------------------------------------------------------
    private async Task<Dictionary<string, Guid>> SeedTagsAsync(CancellationToken cancellationToken)
    {
        if (await _context.Tags.IgnoreQueryFilters().AnyAsync(cancellationToken: cancellationToken))
        {
            return await _context.Tags.IgnoreQueryFilters()
                .ToDictionaryAsync(t => t.Slug, t => t.Id, cancellationToken);
        }

        Tag[] tags =
        [
            new() { Name = "CMS",        Slug = "cms" },
            new() { Name = ".NET",       Slug = "dotnet" },
            new() { Name = "Open Source",Slug = "open-source" },
            new() { Name = "Tutorial",   Slug = "tutorial" },
            new() { Name = "Release",    Slug = "release" },
        ];

        await _context.Tags.AddRangeAsync(tags, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} tags seeded.", tags.Length);

        return tags.ToDictionary(t => t.Slug, t => t.Id);
    }

    // -------------------------------------------------------------------------
    // Static Pages (Home, About, Contact, Privacy)
    // -------------------------------------------------------------------------
    private async Task SeedPagesAsync(Guid authorId, CancellationToken cancellationToken)
    {
        if (await _context.Contents.IgnoreQueryFilters()
                .AnyAsync(c => c.ContentType == "page", cancellationToken))
            return;

        Content[] pages =
        [
            new()
            {
                Title       = "Home",
                Slug        = "home",
                ContentType = "page",
                Status      = ContentStatus.Published,
                PublishedAt = DateTime.UtcNow,
                AuthorId    = authorId == Guid.Empty ? null : authorId,
                Excerpt     = "Welcome to EnterpriseCMS.",
                Body        = "<h1>Welcome to EnterpriseCMS</h1><p>A powerful, extensible content management platform built on .NET 9.</p>",
                MetaTitle       = "Home | EnterpriseCMS",
                MetaDescription = "Welcome to EnterpriseCMS — enterprise-grade content management on .NET 9.",
                SortOrder   = 1,
            },
            new()
            {
                Title       = "About",
                Slug        = "about",
                ContentType = "page",
                Status      = ContentStatus.Published,
                PublishedAt = DateTime.UtcNow,
                AuthorId    = authorId == Guid.Empty ? null : authorId,
                Excerpt     = "Learn more about EnterpriseCMS.",
                Body        = "<h1>About</h1><p>EnterpriseCMS is an open-source CMS built for teams that need flexibility, performance, and security.</p>",
                MetaTitle       = "About | EnterpriseCMS",
                MetaDescription = "Learn about the EnterpriseCMS platform.",
                SortOrder   = 2,
            },
            new()
            {
                Title       = "Contact",
                Slug        = "contact",
                ContentType = "page",
                Status      = ContentStatus.Published,
                PublishedAt = DateTime.UtcNow,
                AuthorId    = authorId == Guid.Empty ? null : authorId,
                Body        = "<h1>Contact Us</h1><p>Get in touch with the EnterpriseCMS team.</p>",
                MetaTitle       = "Contact | EnterpriseCMS",
                MetaDescription = "Contact the EnterpriseCMS team.",
                SortOrder   = 3,
            },
            new()
            {
                Title       = "Privacy Policy",
                Slug        = "privacy-policy",
                ContentType = "page",
                Status      = ContentStatus.Published,
                PublishedAt = DateTime.UtcNow,
                AuthorId    = authorId == Guid.Empty ? null : authorId,
                Body        = "<h1>Privacy Policy</h1><p>Your privacy is important to us. This policy outlines how we collect and use data.</p>",
                MetaTitle       = "Privacy Policy | EnterpriseCMS",
                MetaDescription = "Read the EnterpriseCMS privacy policy.",
                SortOrder   = 4,
            },
        ];

        await _context.Contents.AddRangeAsync(pages, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} pages seeded.", pages.Length);
    }

    // -------------------------------------------------------------------------
    // Blog Posts
    // -------------------------------------------------------------------------
    private async Task SeedPostsAsync(
        Guid authorId,
        Dictionary<string, Guid> categoryIds,
        Dictionary<string, Guid> tagIds,
        CancellationToken cancellationToken)
    {
        if (await _context.Contents.IgnoreQueryFilters()
                .AnyAsync(c => c.ContentType == "post", cancellationToken))
            return;

        var post1 = new Content
        {
            Title       = "Welcome to EnterpriseCMS",
            Slug        = "welcome-to-enterprisecms",
            ContentType = "post",
            Status      = ContentStatus.Published,
            PublishedAt = DateTime.UtcNow.AddDays(-3),
            AuthorId    = authorId == Guid.Empty ? null : authorId,
            Excerpt     = "Introducing EnterpriseCMS — an open-source, enterprise-grade CMS on .NET 9.",
            Body        = "<h2>Getting Started</h2><p>EnterpriseCMS provides a full-featured admin panel, multi-tenant support, and a clean REST API out of the box. Explore the documentation to learn more.</p>",
            MetaTitle       = "Welcome to EnterpriseCMS",
            MetaDescription = "Introducing EnterpriseCMS — built on .NET 9.",
            SortOrder   = 1,
        };

        var post2 = new Content
        {
            Title       = "Building Content with the Block Editor",
            Slug        = "building-content-with-block-editor",
            ContentType = "post",
            Status      = ContentStatus.Published,
            PublishedAt = DateTime.UtcNow.AddDays(-1),
            AuthorId    = authorId == Guid.Empty ? null : authorId,
            Excerpt     = "Learn how to create rich content using the EnterpriseCMS block editor.",
            Body        = "<h2>Block Editor</h2><p>The block-based editor lets you compose pages and posts from reusable content blocks — headings, rich text, images, embeds and more.</p>",
            MetaTitle       = "Block Editor Guide | EnterpriseCMS",
            MetaDescription = "How to use the block editor in EnterpriseCMS.",
            SortOrder   = 2,
        };

        var post3 = new Content
        {
            Title       = "How to Extend EnterpriseCMS with Plugins",
            Slug        = "extending-with-plugins",
            ContentType = "post",
            Status      = ContentStatus.Draft,
            AuthorId    = authorId == Guid.Empty ? null : authorId,
            Excerpt     = "EnterpriseCMS is designed with extensibility in mind. This post explains the plugin system.",
            Body        = "<h2>Plugin System</h2><p>Drop a plugin assembly into the Plugins folder and the CMS discovers it automatically at startup using the EnterpriseCMS.Plugins.Core contract.</p>",
            MetaTitle       = "Plugin Development | EnterpriseCMS",
            MetaDescription = "Extend EnterpriseCMS using the plugin system.",
            SortOrder   = 3,
        };

        await _context.Contents.AddRangeAsync([post1, post2, post3], cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Assign categories
        if (categoryIds.TryGetValue("tutorials", out var tutCatId))
        {
            _context.ContentCategories.Add(new ContentCategory { ContentId = post2.Id, CategoryId = tutCatId });
            _context.ContentCategories.Add(new ContentCategory { ContentId = post3.Id, CategoryId = tutCatId });
        }
        if (categoryIds.TryGetValue("news", out var newsCatId))
            _context.ContentCategories.Add(new ContentCategory { ContentId = post1.Id, CategoryId = newsCatId });

        // Assign tags
        if (tagIds.TryGetValue("cms", out var cmsTagId))
        {
            _context.ContentTags.Add(new ContentTag { ContentId = post1.Id, TagId = cmsTagId });
            _context.ContentTags.Add(new ContentTag { ContentId = post2.Id, TagId = cmsTagId });
        }
        if (tagIds.TryGetValue("dotnet", out var dotnetTagId))
        {
            _context.ContentTags.Add(new ContentTag { ContentId = post1.Id, TagId = dotnetTagId });
            _context.ContentTags.Add(new ContentTag { ContentId = post3.Id, TagId = dotnetTagId });
        }
        if (tagIds.TryGetValue("tutorial", out var tutTagId))
        {
            _context.ContentTags.Add(new ContentTag { ContentId = post2.Id, TagId = tutTagId });
            _context.ContentTags.Add(new ContentTag { ContentId = post3.Id, TagId = tutTagId });
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("3 sample blog posts seeded.");
    }

    // -------------------------------------------------------------------------
    // Navigation Menu
    // -------------------------------------------------------------------------
    private async Task SeedNavigationMenuAsync(CancellationToken cancellationToken)
    {
        if (await _context.Menus.AnyAsync(cancellationToken: cancellationToken))
            return;

        var menu = new Menu
        {
            Name     = "Primary Navigation",
            Slug     = "primary",
            Location = "primary",
        };

        await _context.Menus.AddAsync(menu, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        MenuItem[] items =
        [
            new() { MenuId = menu.Id, Label = "Home",    Url = "/",        SortOrder = 1 },
            new() { MenuId = menu.Id, Label = "Blog",    Url = "/blog",    SortOrder = 2 },
            new() { MenuId = menu.Id, Label = "About",   Url = "/about",   SortOrder = 3 },
            new() { MenuId = menu.Id, Label = "Contact", Url = "/contact", SortOrder = 4 },
        ];

        await _context.MenuItems.AddRangeAsync(items, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Primary navigation menu seeded.");
    }
}
