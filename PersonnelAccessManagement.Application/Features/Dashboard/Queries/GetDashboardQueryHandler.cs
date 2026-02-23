using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Features.Dashboard.Dtos;
using PersonnelAccessManagement.Application.Features.Events.Dtos;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Dashboard.Queries;

public sealed class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IRepository<Personnel> _personnels;
    private readonly IRepository<Rule> _rules;
    private readonly IRepository<Event> _events;
    private readonly IRepository<Job> _jobs;

    public GetDashboardQueryHandler(
        IRepository<Personnel> personnels,
        IRepository<Rule> rules,
        IRepository<Job> jobs,
        IRepository<Event> events)
    {
        _personnels = personnels;
        _rules = rules;
        _events = events;
        _jobs = jobs;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        // Stats
        var totalPersonnel = await _personnels.QueryAsNoTracking()
            .CountAsync(p => !p.IsDeleted, ct);

        var totalRules = await _rules.QueryAsNoTracking()
            .CountAsync(r => !r.IsDeleted, ct);

        var activeRules = await _rules.QueryAsNoTracking()
            .CountAsync(r => !r.IsDeleted && r.IsActive, ct);

        var totalEvents = await _events.QueryAsNoTracking()
            .CountAsync(ct);

        // Completed event = job tamamlandı sayısı
        var totalJobs = await _jobs.QueryAsNoTracking()
            .CountAsync(e => !e.IsDeleted, ct);

        var stats = new DashboardStatsDto(
            totalPersonnel, totalRules, activeRules, totalEvents, totalJobs
        );

        // Son 20 event
        var recentEvents = await _events.QueryAsNoTracking()
            .OrderByDescending(e => e.OccurredAt)
            .Take(20)
            .Select(e => new EventDto(
                e.Id,
                e.EventType.ToString(),
                e.SourceId,
                e.SourceDetail,
                e.CorrelationId,
                e.OccurredAt,
                e.TotalCount,
                e.SuccessCount,
                e.FailCount,
                e.IsCompleted,
                e.CreatedAt
            ))
            .ToListAsync(ct);
        
         var recentJobs = await _jobs.QueryAsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .Take(20)
            .Select(j => new JobDto(
                j.Id,
                j.JobType.ToString(),
                j.Status,
                j.TotalCount,
                j.SuccessCount,
                j.FailureCount,
                j.CreatedAt,
                j.UpdatedAt
            ))
            .ToListAsync(ct);

        return new DashboardDto(stats, recentEvents, recentJobs);
    }
}