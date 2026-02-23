using MediatR;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;

namespace PersonnelAccessManagement.Application.Features.Jobs.Queries;

public sealed record GetJobLogsQuery(Guid JobId) : IRequest<List<JobLogDto>>;