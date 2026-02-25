using DotNetCore.CAP;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
    Task<IDisposable> BeginTransactionAsync(ICapPublisher capPublisher, CancellationToken ct = default);

}