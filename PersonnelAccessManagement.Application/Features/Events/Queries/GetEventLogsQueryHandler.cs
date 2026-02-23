using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Events.Dtos;
using EventLog = PersonnelAccessManagement.Domain.Entities.EventLog;

namespace PersonnelAccessManagement.Application.Features.Events.Queries;

public sealed class GetEventLogsQueryHandler : IRequestHandler<GetEventLogsQuery, List<EventLogDto>>
{
    private readonly IRepository<EventLog> _logs;

    public GetEventLogsQueryHandler(IRepository<EventLog> logs) => _logs = logs;

    public async Task<List<EventLogDto>> Handle(GetEventLogsQuery request, CancellationToken ct)
    {
        return await _logs.QueryAsNoTracking()
            .Where(l => l.EventId == request.EventId)
            .OrderBy(l => l.CreatedAt)
            .Select(l => new EventLogDto(
                l.Id, l.EventId, l.EmployeeNo, l.PersonnelName,
                l.RoleId, l.RoleName, l.Action, l.Status.ToString(),
                l.Error, l.CreatedAt
            ))
            .ToListAsync(ct);
    }
}