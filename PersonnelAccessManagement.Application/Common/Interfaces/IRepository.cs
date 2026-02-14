namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);

    IQueryable<TEntity> Query();               
    IQueryable<TEntity> QueryAsNoTracking();  
}