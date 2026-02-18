namespace PersonnelAccessManagement.Application.Features.Dashboard.Dtos;

public sealed record DashboardStatsDto(
    int TotalPersonnel,
    int TotalRules,
    int ActiveRules,
    int TotalEvents,
    int TotalJobs
);