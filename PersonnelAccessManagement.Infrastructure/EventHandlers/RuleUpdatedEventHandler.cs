using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Rules.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class RuleUpdatedEventHandler : ICapSubscribe
{
    private readonly IPersonnelRoleService _roleService;
    private readonly ILogger<RuleUpdatedEventHandler> _logger;

    public RuleUpdatedEventHandler(
        IPersonnelRoleService roleService,
        ILogger<RuleUpdatedEventHandler> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.RuleUpdated)]
    public async Task HandleAsync(RuleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "RuleUpdated received — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
            @event.RuleId, @event.CorrelationId);

        try
        {
            await _roleService.ApplyUpdatedRuleToMatchingPersonnelAsync(
                @event.RuleId, @event.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "RuleUpdated processing failed — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
                @event.RuleId, @event.CorrelationId);
            throw;
        }
    }
}