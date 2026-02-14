using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Persistence.DbContexts;

namespace PersonnelAccessManagement.Persistence.Repositories;

public sealed class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly PersonnelAccessManagementDbContext _db;
    private readonly DbSet<TEntity> _set;

    public EfRepository(PersonnelAccessManagementDbContext db)
    {
        _db = db;
        _set = db.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => _set.AsQueryable();

    public IQueryable<TEntity> QueryAsNoTracking() => _set.AsNoTracking();

    public Task AddAsync(TEntity entity, CancellationToken ct = default)
        => _set.AddAsync(entity, ct).AsTask();

    public void Update(TEntity entity) => _set.Update(entity);

    public void Remove(TEntity entity) => _set.Remove(entity);
}