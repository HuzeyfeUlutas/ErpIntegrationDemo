using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Personnels.Dtos;
using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Personnels.Queries;

public sealed class ListPersonnelQueryHandler 
    : IRequestHandler<ListPersonnelQuery, PagedQueryResult<IEnumerable<PersonnelDto>>>
{
    private readonly IRepository<Personnel> _personnels;

    public ListPersonnelQueryHandler(IRepository<Personnel> personnels)
    {
        _personnels = personnels;
    }

    public async Task<PagedQueryResult<IEnumerable<PersonnelDto>>> Handle(ListPersonnelQuery request, CancellationToken ct)
    {
        var search = request.Filter.Search?.Trim();
        var hasNumericSearch = decimal.TryParse(search, out var empNo);

        var q = _personnels.QueryAsNoTracking()
            .Include(p => p.Roles)
            .Where(p => !p.IsDeleted)
            .FilterIf(p => p.Campus == request.Filter.Campus, request.Filter.Campus is not null)
            .FilterIf(p => p.Title == request.Filter.Title, request.Filter.Title is not null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(p =>
                (hasNumericSearch && p.EmployeeNo == empNo) ||
                EF.Functions.Like(p.FullName, $"{search}%"));
        }

        var rowCount = await q.CountAsync(ct);

        var entities = await q
            .OrderByDescending(p => p.CreatedAt)
            .GetPaged(request.Filter)
            .ToListAsync(ct);

        var dtos = entities.Select(p => new PersonnelDto(
            p.EmployeeNo,
            p.FullName,
            p.Campus,
            p.Title,
            p.Roles.Select(r => new RoleDto(r.Id, r.Name)).ToList()
        )).ToList();

        return dtos.ToPagedQueryResult(request.Filter, rowCount);
    }
}