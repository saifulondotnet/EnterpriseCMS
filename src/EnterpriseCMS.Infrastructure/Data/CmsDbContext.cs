using EnterpriseCMS.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCMS.Infrastructure.Data;

public class CmsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
    Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>,
    ApplicationUserRole,
    Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>,
    Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>,
    Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>
{
    private readonly Guid _tenantId;

    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }

    public DbSet<Content> Contents => Set<Content>();
    public DbSet<ContentVersion> ContentVersions => Set<ContentVersion>();
    public DbSet<ContentMeta> ContentMeta => Set<ContentMeta>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<MediaFolder> MediaFolders => Set<MediaFolder>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ContentTag> ContentTags => Set<ContentTag>();
    public DbSet<ContentCategory> ContentCategories => Set<ContentCategory>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Widget> Widgets => Set<Widget>();
    public DbSet<Redirect> Redirects => Set<Redirect>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Soft-delete global query filter
        builder.Entity<Content>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<MediaAsset>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Tag>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Setting>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContentVersion>().HasQueryFilter(e => !e.IsDeleted);

        // Content
        builder.Entity<Content>(e => {
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasIndex(c => new { c.TenantId, c.Status });
            e.HasIndex(c => new { c.TenantId, c.Status, c.PublishedAt });
            e.HasOne(c => c.Author).WithMany().HasForeignKey(c => c.AuthorId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Parent).WithMany(c => c.Children).HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.ClientSetNull);
            e.HasMany(c => c.Versions).WithOne(v => v.Content).HasForeignKey(v => v.ContentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(c => c.Meta).WithOne(m => m.Content).HasForeignKey(m => m.ContentId).OnDelete(DeleteBehavior.Cascade);
        });

        // MediaAsset index
        builder.Entity<MediaAsset>(e => {
            e.HasIndex(m => new { m.TenantId, m.FolderId });
        });

        // AuditLog index
        builder.Entity<AuditLog>(e => {
            e.HasIndex(a => new { a.TenantId, a.CreatedAt });
        });

        // Many-to-many
        builder.Entity<ContentTag>().HasKey(ct => new { ct.ContentId, ct.TagId });
        builder.Entity<ContentTag>()
            .HasOne(ct => ct.Content).WithMany(c => c.ContentTags).HasForeignKey(ct => ct.ContentId);
        builder.Entity<ContentTag>()
            .HasOne(ct => ct.Tag).WithMany(t => t.ContentTags).HasForeignKey(ct => ct.TagId);

        builder.Entity<ContentCategory>().HasKey(cc => new { cc.ContentId, cc.CategoryId });
        builder.Entity<ContentCategory>()
            .HasOne(cc => cc.Content).WithMany(c => c.ContentCategories).HasForeignKey(cc => cc.ContentId);
        builder.Entity<ContentCategory>()
            .HasOne(cc => cc.Category).WithMany(c => c.ContentCategories).HasForeignKey(cc => cc.CategoryId);

        // Identity UserRole join
        builder.Entity<ApplicationUserRole>(e => {
            e.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            e.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
        });

        // Menu
        builder.Entity<MenuItem>()
            .HasOne(mi => mi.Parent).WithMany(mi => mi.Children).HasForeignKey(mi => mi.ParentId).OnDelete(DeleteBehavior.Restrict);

        // Category self-reference
        builder.Entity<Category>()
            .HasOne(c => c.Parent).WithMany(c => c.Children).HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.ClientSetNull);

        // MediaFolder self-reference
        builder.Entity<MediaFolder>()
            .HasOne(f => f.Parent).WithMany(f => f.Children).HasForeignKey(f => f.ParentId).OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<Widget>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Redirect>().HasQueryFilter(e => !e.IsDeleted);
    }
}
