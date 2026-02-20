using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Constants;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Enums;
using PersonnelAccessManagement.Domain.Events;
using PersonnelAccessManagement.Infrastructure.Kafka.Abstractions;

namespace PersonnelAccessManagement.Infrastructure.Kafka;

public sealed class KafkaCapBridge : IKafkaCapBridge
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaCapBridge> _logger;

    private static readonly Dictionary<PersonnelEventType, string> TopicMap = new()
    {
        [PersonnelEventType.Hired] = CapTopics.Hired,
        [PersonnelEventType.Terminated] = CapTopics.Terminated,
        [PersonnelEventType.PositionChanged] = CapTopics.PositionChanged,
    };

    public KafkaCapBridge(
        IServiceScopeFactory scopeFactory,
        ILogger<KafkaCapBridge> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public bool TryGetCapTopic(PersonnelLifecycleIntegrationEvent evt, out string capTopic)
        => TopicMap.TryGetValue(evt.EventType, out capTopic!);

    public async Task PublishAsync(
        PersonnelLifecycleIntegrationEvent evt,
        string capTopic,
        CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var capPublisher = scope.ServiceProvider.GetRequiredService<ICapPublisher>();

        var integrationEvent = evt with { PublishedAtUtc = DateTimeOffset.UtcNow };

        using var transaction = await uow.BeginTransactionAsync(capPublisher, ct);
        await capPublisher.PublishAsync(capTopic, integrationEvent, cancellationToken: ct);
        await uow.CommitTransactionAsync(ct);

        _logger.LogInformation(
            "Kafka → CAP published — EventId: {EventId}, Topic: {CapTopic}, EmployeeNo: {EmployeeNo}",
            evt.EventId, capTopic, evt.EmployeeNo);
    }
}