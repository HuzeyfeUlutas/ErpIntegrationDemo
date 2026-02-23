using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Events.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Events.Queries;

public sealed class ListEventsQueryHandler
    : IRequestHandler<ListEventsQuery, PagedQueryResult<IEnumerable<EventDto>>>
{
    private readonly IRepository<Event> _events;

    public ListEventsQueryHandler(IRepository<Event> events) => _events = events;

    public async Task<PagedQueryResult<IEnumerable<EventDto>>> Handle(
        ListEventsQuery request, CancellationToken ct)
    {
        var f = request.Filter;
        var search = f.Search?.Trim();

        var q = _events.QueryAsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(e => e.SourceId.Contains(search) || e.CorrelationId.Contains(search));

        if (!string.IsNullOrWhiteSpace(f.EventType))
            q = q.Where(e => e.EventType.ToString() == f.EventType);

        if (f.IsCompleted.HasValue)
            q = q.Where(e => e.IsCompleted == f.IsCompleted.Value);

        var rowCount = await q.CountAsync(ct);

        var entities = await q
            .OrderByDescending(e => e.OccurredAt)
            .GetPaged(f)
            .ToListAsync(ct);

        var dtos = entities.Select(e => new EventDto(
            e.Id, e.EventType.ToString(), e.SourceId, e.SourceDetail,
            e.CorrelationId, e.OccurredAt, e.TotalCount, e.SuccessCount,
            e.FailCount, e.IsCompleted, e.CreatedAt
        )).ToList();

        return dtos.ToPagedQueryResult(f, rowCount);
    }
}