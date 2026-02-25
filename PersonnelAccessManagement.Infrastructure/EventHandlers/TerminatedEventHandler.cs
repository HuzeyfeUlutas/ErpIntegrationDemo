using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Options;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;
using PersonnelAccessManagement.Domain.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class TerminatedEventHandler : ICapSubscribe
{
    private readonly IRepository<PersonnelScheduledAction> _scheduledActionRepo;
    private readonly IRepository<Personnel> _personnelRepo;
    private readonly IRepository<Role> _roleRepo;
    private readonly IUnitOfWork _uow;
    private readonly PersonnelRoleOptions _roleOptions;
    private readonly ILogger<TerminatedEventHandler> _logger;

    public TerminatedEventHandler(
        IRepository<PersonnelScheduledAction> scheduledActionRepo,
        IRepository<Personnel> personnelRepo,
        IRepository<Role> roleRepo,
        IUnitOfWork uow,
        IOptions<PersonnelRoleOptions> roleOptions,
        ILogger<TerminatedEventHandler> logger)
    {
        _scheduledActionRepo = scheduledActionRepo;
        _personnelRepo = personnelRepo;
        _roleRepo = roleRepo;
        _uow = uow;
        _roleOptions = roleOptions.Value;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.Terminated)]
    public async Task HandleAsync(PersonnelLifecycleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "CAP received TERMINATED — EmployeeNo: {EmployeeNo}, EffectiveDate: {Date}, EventId: {EventId}",
            @event.EmployeeNo, @event.EffectiveDate, @event.EventId);
        
        var exists = await _scheduledActionRepo.QueryAsNoTracking()
            .AnyAsync(x => x.EventId == @event.EventId);

        if (exists)
        {
            _logger.LogWarning("Duplicate event {EventId} for TERMINATED, skipping.", @event.EventId);
            return;
        }
        
        var alreadyScheduled = await _scheduledActionRepo.QueryAsNoTracking()
            .AnyAsync(x => x.EmployeeNo == @event.EmployeeNo
                           && x.ActionType == ScheduledActionType.Terminate
                           && x.Status == ScheduledActionStatus.Pending);

        if (alreadyScheduled)
        {
            _logger.LogWarning(
                "Personnel {EmployeeNo} already has a pending TERMINATE action, skipping EventId: {EventId}.",
                @event.EmployeeNo, @event.EventId);
            return;
        }
        
        if (!decimal.TryParse(@event.EmployeeNo, out var empNo))
        {
            _logger.LogError("Invalid EmployeeNo format: {EmployeeNo}, EventId: {EventId}",
                @event.EmployeeNo, @event.EventId);
            return;
        }
        
        var personnel = await _personnelRepo.Query()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.EmployeeNo == decimal.Parse(@event.EmployeeNo));

        if (personnel is null)
        {
            _logger.LogWarning(
                "Personnel {EmployeeNo} not found, scheduling termination only.", @event.EmployeeNo);
        }
        else
        {
            var activeRole = personnel.Roles.FirstOrDefault(r => r.Id == _roleOptions.DefaultHireRoleId);

            if (activeRole is not null)
            {
                personnel.RemoveRole(activeRole.Id);
                _logger.LogInformation("Removed DefaultHireRole {RoleId} from {EmployeeNo}", activeRole.Id, @event.EmployeeNo);
            }
            
            var exitingRole = await _roleRepo.Query()
                .FirstOrDefaultAsync(r => r.Id == _roleOptions.ExitingRoleId);

            if (exitingRole is not null)
            {
                personnel.AddRole(exitingRole);
                _logger.LogInformation("Assigned ExitingRole {RoleId} to {EmployeeNo}", exitingRole.Id, @event.EmployeeNo);
            }
            else
            {
                _logger.LogError("ExitingRole {RoleId} not found in database!", _roleOptions.ExitingRoleId);
            }
        }
        
        var action = new PersonnelScheduledAction
        {
            EventId = @event.EventId,
            EmployeeNo = @event.EmployeeNo,
            ActionType = ScheduledActionType.Terminate,
            EffectiveDate = @event.EffectiveDate,
            CorrelationId = @event.CorrelationId,
            Status = ScheduledActionStatus.Pending
        };

        await _scheduledActionRepo.AddAsync(action);
        await _uow.SaveChangesAsync();

        _logger.LogInformation(
            "Scheduled TERMINATE for {EmployeeNo} on {EffectiveDate} | EventId: {EventId}",
            @event.EmployeeNo, @event.EffectiveDate, @event.EventId);
    }
}