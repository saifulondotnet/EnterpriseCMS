using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Interfaces;
using EnterpriseCMS.Infrastructure.Data;

namespace EnterpriseCMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CmsDbContext _db;
    private bool _disposed;

    public UnitOfWork(CmsDbContext db)
    {
        _db = db;
        Contents = new BaseRepository<Content>(db);
        ContentVersions = new BaseRepository<ContentVersion>(db);
        ContentMeta = new BaseRepository<ContentMeta>(db);
        MediaAssets = new BaseRepository<MediaAsset>(db);
        MediaFolders = new BaseRepository<MediaFolder>(db);
        Categories = new BaseRepository<Category>(db);
        Tags = new BaseRepository<Tag>(db);
        Settings = new BaseRepository<Setting>(db);
        Menus = new BaseRepository<Menu>(db);
        MenuItems = new BaseRepository<MenuItem>(db);
        AuditLogs = new BaseRepository<AuditLog>(db);
        Widgets = new BaseRepository<Widget>(db);
        Redirects = new BaseRepository<Redirect>(db);
    }

    public IRepository<Content> Contents { get; }
    public IRepository<ContentVersion> ContentVersions { get; }
    public IRepository<ContentMeta> ContentMeta { get; }
    public IRepository<MediaAsset> MediaAssets { get; }
    public IRepository<MediaFolder> MediaFolders { get; }
    public IRepository<Category> Categories { get; }
    public IRepository<Tag> Tags { get; }
    public IRepository<Setting> Settings { get; }
    public IRepository<Menu> Menus { get; }
    public IRepository<MenuItem> MenuItems { get; }
    public IRepository<AuditLog> AuditLogs { get; }
    public IRepository<Widget> Widgets { get; }
    public IRepository<Redirect> Redirects { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public void Dispose()
    {
        if (!_disposed) { _db.Dispose(); _disposed = true; }
        GC.SuppressFinalize(this);
    }
}
