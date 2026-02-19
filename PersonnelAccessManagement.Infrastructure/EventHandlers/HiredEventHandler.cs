using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;
using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class HiredEventHandler : ICapSubscribe
{
    private readonly IRepository<PersonnelScheduledAction> _scheduledActionRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<HiredEventHandler> _logger;

    public HiredEventHandler(
        IRepository<PersonnelScheduledAction> scheduledActionRepo,
        IUnitOfWork uow,
        ILogger<HiredEventHandler> logger)
    {
        _scheduledActionRepo = scheduledActionRepo;
        _uow = uow;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.Hired)]
    public async Task HandleAsync(PersonnelLifecycleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "CAP received HIRED — EmployeeNo: {EmployeeNo}, EffectiveDate: {Date}, EventId: {EventId}",
            @event.EmployeeNo, @event.EffectiveDate, @event.EventId);

        // Idempotency
        var exists = await _scheduledActionRepo.QueryAsNoTracking()
            .AnyAsync(x => x.EventId == @event.EventId);

        if (exists)
        {
            _logger.LogWarning("Duplicate event {EventId} for HIRED, skipping.", @event.EventId);
            return;
        }

        var action = new PersonnelScheduledAction
        {
            EventId = @event.EventId,
            EmployeeNo = @event.EmployeeNo,
            ActionType = ScheduledActionType.Hire,
            EffectiveDate = @event.EffectiveDate,
            CorrelationId = @event.CorrelationId,
            Status = ScheduledActionStatus.Pending
        };

        await _scheduledActionRepo.AddAsync(action);
        await _uow.SaveChangesAsync();

        _logger.LogInformation(
            "Scheduled HIRE for {EmployeeNo} on {EffectiveDate} | EventId: {EventId}",
            @event.EmployeeNo, @event.EffectiveDate, @event.EventId);
    }
}