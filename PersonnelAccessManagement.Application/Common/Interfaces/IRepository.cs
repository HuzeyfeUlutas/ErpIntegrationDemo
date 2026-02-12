namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);

    IQueryable<TEntity> Query();               
    IQueryable<TEntity> QueryAsNoTracking();  
}