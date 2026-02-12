using Microsoft.EntityFrameworkCore.Storage;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Persistence.DbContexts;

namespace PersonnelAccessManagement.Persistence.EfUnitOfWork;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly PersonnelAccessManagementDbContext _db;
    private IDbContextTransaction? _tx;

    public EfUnitOfWork(PersonnelAccessManagementDbContext  db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _tx ??= await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;
        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }
}