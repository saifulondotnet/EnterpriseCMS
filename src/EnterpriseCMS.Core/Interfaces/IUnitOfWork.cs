using EnterpriseCMS.Core.Entities;

namespace EnterpriseCMS.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Content> Contents { get; }
    IRepository<ContentVersion> ContentVersions { get; }
    IRepository<ContentMeta> ContentMeta { get; }
    IRepository<MediaAsset> MediaAssets { get; }
    IRepository<MediaFolder> MediaFolders { get; }
    IRepository<Category> Categories { get; }
    IRepository<Tag> Tags { get; }
    IRepository<Setting> Settings { get; }
    IRepository<Menu> Menus { get; }
    IRepository<MenuItem> MenuItems { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<Widget> Widgets { get; }
    IRepository<Redirect> Redirects { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
