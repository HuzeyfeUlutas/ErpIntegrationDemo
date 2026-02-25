using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Roles.Queries;

public sealed class ListRolesQueryHandler 
    : IRequestHandler<ListRolesQuery, PagedQueryResult<IEnumerable<RoleDto>>>
{
    private readonly IRepository<Role> _roles;

    public ListRolesQueryHandler(IRepository<Role> roles)
    {
        _roles = roles;
    }

    public async Task<PagedQueryResult<IEnumerable<RoleDto>>> Handle(ListRolesQuery request, CancellationToken ct)
    {
        var f = request.Filter;
        var name = f.Name?.Trim();

        var q = _roles.QueryAsNoTracking()
            .Where(r => !r.IsDeleted)
            .FilterIf(r => EF.Functions.Like(r.Name, $"{name}%"), !string.IsNullOrWhiteSpace(name));

        var rowCount = await q.CountAsync(ct);

        var items = await q
            .OrderBy(r => r.Name)
            .GetPaged(f)
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToListAsync(ct);

        return items.ToPagedQueryResult(f, rowCount);
    }
}