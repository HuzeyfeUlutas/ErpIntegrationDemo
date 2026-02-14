using Microsoft.EntityFrameworkCore.Storage;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Persistence.DbContexts;

namespace PersonnelAccessManagement.Persistence.EfUnitOfWork;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly PersonnelAccessManagementDbContext _db;
    private IDbContextTransaction? _currentTransaction;

    public EfUnitOfWork(PersonnelAccessManagementDbContext db) => _db = db;

    public bool HasActiveTransaction => _currentTransaction is not null;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _db.Database.BeginTransactionAsync(ct);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await _db.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
        }
        catch
        {
            await RollbackTransactionAsync(ct);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException("No active transaction to rollback.");
        }

        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        DisposeTransactionAsync().GetAwaiter().GetResult();
    }
}