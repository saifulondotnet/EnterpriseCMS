using System.Linq.Expressions;
using EnterpriseCMS.Core.Interfaces;
using EnterpriseCMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCMS.Infrastructure.Repositories;

public class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly CmsDbContext _db;
    protected readonly DbSet<T> _set;

    public BaseRepository(CmsDbContext db) { _db = db; _set = db.Set<T>(); }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _set.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.ToListAsync(ct);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _set.Where(predicate).ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate == null ? await _set.CountAsync(ct) : await _set.CountAsync(predicate, ct);

    public IQueryable<T> Query() => _set.AsQueryable();
}
