using MediatR;
using PersonnelAccessManagement.Application.Features.Dashboard.Dtos;

namespace PersonnelAccessManagement.Application.Features.Dashboard.Queries;

public sealed record GetDashboardQuery : IRequest<DashboardDto>;