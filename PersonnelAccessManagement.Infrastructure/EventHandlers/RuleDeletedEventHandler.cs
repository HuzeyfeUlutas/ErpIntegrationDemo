using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Rules.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class RuleDeletedEventHandler : ICapSubscribe
{
    private readonly IPersonnelRoleService _roleService;
    private readonly ILogger<RuleDeletedEventHandler> _logger;

    public RuleDeletedEventHandler(
        IPersonnelRoleService roleService,
        ILogger<RuleDeletedEventHandler> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.RuleDeleted)]
    public async Task HandleAsync(RuleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "RuleDeleted received — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
            @event.RuleId, @event.CorrelationId);

        try
        {
            await _roleService.ApplyDeletedRuleToMatchingPersonnelAsync(
                @event.RuleId, @event.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "RuleDeleted processing failed — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
                @event.RuleId, @event.CorrelationId);
            throw;
        }
    }
}