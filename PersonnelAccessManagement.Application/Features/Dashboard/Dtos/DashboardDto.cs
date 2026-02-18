using PersonnelAccessManagement.Application.Features.Events.Dtos;

namespace PersonnelAccessManagement.Application.Features.Dashboard.Dtos;

public sealed record DashboardDto(
    DashboardStatsDto Stats,
    IReadOnlyList<EventDto> RecentEvents
);