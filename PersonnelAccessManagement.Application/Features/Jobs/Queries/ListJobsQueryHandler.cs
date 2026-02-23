using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Jobs.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Jobs.Queries;

public sealed class ListJobsQueryHandler
    : IRequestHandler<ListJobsQuery, PagedQueryResult<IEnumerable<JobDto>>>
{
    private readonly IRepository<Job> _jobs;

    public ListJobsQueryHandler(IRepository<Job> jobs) => _jobs = jobs;

    public async Task<PagedQueryResult<IEnumerable<JobDto>>> Handle(
        ListJobsQuery request, CancellationToken ct)
    {
        var f = request.Filter;
        var search = f.Search?.Trim();

        var q = _jobs.QueryAsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(f.Status))
            q = q.Where(j => j.Status == f.Status);

        if (!string.IsNullOrWhiteSpace(f.JobType))
            q = q.Where(j => j.JobType.ToString() == f.JobType);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(j => j.Id.ToString().Contains(search));

        var rowCount = await q.CountAsync(ct);

        var entities = await q
            .OrderByDescending(j => j.CreatedAt)
            .GetPaged(f)
            .ToListAsync(ct);

        var dtos = entities.Select(j => new JobDto(
            j.Id, j.JobType.ToString(), j.Status,
            j.TotalCount, j.SuccessCount, j.FailureCount,
            j.CreatedAt, j.UpdatedAt
        )).ToList();

        return dtos.ToPagedQueryResult(f, rowCount);
    }
}