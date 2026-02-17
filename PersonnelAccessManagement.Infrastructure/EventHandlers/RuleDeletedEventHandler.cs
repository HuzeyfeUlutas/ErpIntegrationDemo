using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Events;

namespace PersonnelAccessManagement.Infrastructure.EventHandlers;

public sealed class RuleDeletedEventHandler : ICapSubscribe
{
    private readonly IPersonnelRoleService _roleService;
    private readonly ILogger<RuleCreatedEventHandler> _logger;

    public RuleDeletedEventHandler(
        IPersonnelRoleService roleService,
        ILogger<RuleCreatedEventHandler> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [CapSubscribe(CapTopics.RuleDeleted)]
    public async Task HandleAsync(RuleIntegrationEvent @event)
    {
        _logger.LogInformation(
            "RuleCreated received — RuleId: {RuleId}, CorrelationId: {CorrelationId}",
            @event.RuleId, @event.CorrelationId);
    }
}