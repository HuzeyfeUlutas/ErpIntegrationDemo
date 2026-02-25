using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Rules.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class RuleCreatedEventHandler : ICapSubscribe
{
    private readonly IPersonnelRoleService _roleService;
    private readonly ILogger<RuleCreatedEventHandler> _logger;

    public RuleCreatedEventHandler(
        IPersonnelRoleService roleService,
        ILogger<RuleCreatedEventHandler> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.RuleCreated)]
    public async Task HandleAsync(RuleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "RuleCreated received — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
            @event.RuleId, @event.CorrelationId);

        try
        {
            await _roleService.ApplyCreatedRuleToMatchingPersonnelAsync(
                @event.RuleId, @event.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "RuleCreated processing failed — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
                @event.RuleId, @event.CorrelationId);
            throw;
        }
    }
}