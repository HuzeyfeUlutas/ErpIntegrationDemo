using Middleware.Contracts.Events;

namespace MiddlewareApplication.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(PersonnelLifecycleEvent evt, CancellationToken ct);
}