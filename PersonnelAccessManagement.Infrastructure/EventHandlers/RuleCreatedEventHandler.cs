 using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Events;

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
    public async Task HandleAsync(RuleCreatedIntegrationEvent @event)
    {
        return;
        _logger.LogInformation(
            "RuleCreated received — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
            @event.RuleId, @event.CorrelationId);

        await _roleService.ApplyRuleToMatchingPersonnelAsync(
            @event.RuleId,
            @event.CorrelationId);
    }
}