namespace PersonnelAccessManagement.Application.Features.Rules.Events;

public sealed record RuleIntegrationEvent
{
    public Guid RuleId { get; init; }
    public string CorrelationId { get; init; } = default!;
}