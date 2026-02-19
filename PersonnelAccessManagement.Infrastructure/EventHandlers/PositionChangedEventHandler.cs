using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class PositionChangedEventHandler : ICapSubscribe
{
    private readonly ILogger<PositionChangedEventHandler> _logger;

    public PositionChangedEventHandler(ILogger<PositionChangedEventHandler> logger)
    {
        _logger = logger;
    }

    [CapSubscribe(CapTopics.PositionChanged)]
    public Task HandleAsync(PersonnelLifecycleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "CAP received POSITION CHANGED — EmployeeNo: {EmployeeNo}, EffectiveDate: {Date}, EventId: {EventId}",
            @event.EmployeeNo, @event.EffectiveDate, @event.EventId);

        // TODO: İleride scheduled action'a eklenebilir
        return Task.CompletedTask;
    }
}