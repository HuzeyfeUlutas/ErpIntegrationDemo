using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.KafkaEventLog.Dtos;

namespace PersonnelAccessManagement.Application.Features.KafkaEventLog.Queries;

public sealed class ListKafkaEventLogsQueryHandler
    : IRequestHandler<ListKafkaEventLogsQuery, PagedQueryResult<IEnumerable<KafkaEventLogDto>>>
{
    private readonly IRepository<Domain.Entities.KafkaEventLog> _repo;
    private readonly ILogger<ListKafkaEventLogsQueryHandler> _logger;

    public ListKafkaEventLogsQueryHandler(IRepository<Domain.Entities.KafkaEventLog> repo,  ILogger<ListKafkaEventLogsQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PagedQueryResult<IEnumerable<KafkaEventLogDto>>> Handle(
        ListKafkaEventLogsQuery request, CancellationToken ct)
    {
        var f = request.Filter;
        var search = f.Search?.Trim();

        var q = _repo.QueryAsNoTracking().AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(f.Status))
            q = q.Where(e => e.Status == f.Status);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(e =>
                e.Topic.Contains(search) ||
                (e.MessageKey != null && e.MessageKey.Contains(search)) ||
                (e.MessageValue != null && e.MessageValue.Contains(search)));
        }
        
        if (!string.IsNullOrWhiteSpace(f.EventType))
        {
            q = q.Where(e => e.MessageValue != null && e.MessageValue.Contains(f.EventType));
        }

        var rowCount = await q.CountAsync(ct);

        var entities = await q
            .OrderByDescending(e => e.CreatedAtUtc)
            .GetPaged(f)
            .ToListAsync(ct);

        var dtos = entities.Select(e => MapToDto(e, _logger)).ToList();

        return dtos.ToPagedQueryResult(f, rowCount);
    }

    private static KafkaEventLogDto MapToDto(Domain.Entities.KafkaEventLog entity, ILogger logger)
    {
        string? eventType = null, employeeNo = null, effectiveDate = null,
            occuredAtUtc = null, correlationId = null, eventId = null;

        if (!string.IsNullOrWhiteSpace(entity.MessageValue))
        {
            try
            {
                using var doc = JsonDocument.Parse(entity.MessageValue);
                var root = doc.RootElement;

                eventType = root.TryGetProperty("EventType", out var et) ? et.GetString() : null;
                employeeNo = root.TryGetProperty("EmployeeNo", out var en) ? en.GetString() : null;
                effectiveDate = root.TryGetProperty("EffectiveDate", out var ed) ? ed.GetString() : null;
                occuredAtUtc = root.TryGetProperty("OccuredAtUtc", out var oa) ? oa.GetString() : null;
                correlationId = root.TryGetProperty("CorrelationId", out var ci) ? ci.GetString() : null;
                eventId = root.TryGetProperty("EventId", out var ei) ? ei.GetString() : null;
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, 
                    "KafkaEventLog {Id} — MessageValue JSON parse edilemedi. Value: {Value}", 
                    entity.Id, 
                    entity.MessageValue?[..Math.Min(entity.MessageValue.Length, 200)]);
            }
        }

        return new KafkaEventLogDto(
            entity.Id,
            entity.Topic,
            entity.PartitionNo,
            entity.Offset,
            entity.MessageKey,
            entity.Status,
            entity.RetryCount,
            entity.ErrorMessage,
            entity.ErrorStackTrace,
            entity.CreatedAtUtc,
            eventType,
            employeeNo,
            effectiveDate,
            occuredAtUtc,
            correlationId,
            eventId
        );
    }
}