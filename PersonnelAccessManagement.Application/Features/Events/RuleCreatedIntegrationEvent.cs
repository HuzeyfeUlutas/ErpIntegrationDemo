namespace PersonnelAccessManagement.Application.Features.Events;

public sealed record RuleCreatedIntegrationEvent
{
    public Guid RuleId { get; init; }
    public string CorrelationId { get; init; } = default!;
}