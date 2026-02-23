using PersonnelAccessManagement.Application.Features.Events.Dtos;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;

namespace PersonnelAccessManagement.Application.Features.Dashboard.Dtos;

public sealed record DashboardDto(
    DashboardStatsDto Stats,
    IReadOnlyList<EventDto> RecentEvents,
    IReadOnlyList<JobDto> RecentJobs
);