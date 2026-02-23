using MediatR;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;

namespace PersonnelAccessManagement.Application.Features.Jobs.Queries;

public sealed record ListJobsQuery(JobFilter Filter)
    : IRequest<PagedQueryResult<IEnumerable<JobDto>>>;