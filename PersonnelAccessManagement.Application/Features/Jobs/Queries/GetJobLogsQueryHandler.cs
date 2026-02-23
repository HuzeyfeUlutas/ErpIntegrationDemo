using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Jobs.Queries;

public sealed class GetJobLogsQueryHandler : IRequestHandler<GetJobLogsQuery, List<JobLogDto>>
{
    private readonly IRepository<JobLog> _logs;

    public GetJobLogsQueryHandler(IRepository<JobLog> logs) => _logs = logs;

    public async Task<List<JobLogDto>> Handle(GetJobLogsQuery request, CancellationToken ct)
    {
        return await _logs.QueryAsNoTracking()
            .Where(l => l.JobId == request.JobId)
            .OrderBy(l => l.CreatedAt)
            .Select(l => new JobLogDto(
                l.Id, l.JobId, l.Message, l.Status, l.CreatedAt
            ))
            .ToListAsync(ct);
    }
}