using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using Rule = PersonnelAccessManagement.Domain.Entities.Rule;

namespace PersonnelAccessManagement.Application.Features.Rules.Queries;

public sealed class ListRulesQueryHandler 
    : IRequestHandler<ListRulesQuery, PagedQueryResult<IEnumerable<RuleDto>>>
{
    private readonly IRepository<Rule> _rules;

    public ListRulesQueryHandler(IRepository<Rule> rules)
    {
        _rules = rules;
    }

    public async Task<PagedQueryResult<IEnumerable<RuleDto>>> Handle(ListRulesQuery request, CancellationToken ct)
    {
        var name = request.Filter.Name?.Trim();
        
        var q = _rules.QueryAsNoTracking()
            .Include(r => r.Roles)
            .Where(r => !r.IsDeleted)
            .FilterIf(r => EF.Functions.Like(r.Name, $"{name}%"), !string.IsNullOrWhiteSpace(name))
            .FilterIf(r => r.Campus == request.Filter.Campus, request.Filter.Campus is not null)
            .FilterIf(r => r.Title == request.Filter.Title, request.Filter.Title is not null);
        
        var rowCount = await q.CountAsync(ct);
        
        var pagedQuery = q
            .OrderByDescending(r => r.CreatedAt)
            .GetPaged(request.Filter);

        var entities = await pagedQuery.ToListAsync(ct);
        
        var dtos = entities.Select(r => new RuleDto(
            r.Id,
            r.Name,
            r.Campus,
            r.Title,
            r.IsActive,
            r.Roles.Select(role => new RoleDto(role.Id, role.Name)).ToList()
        )).ToList();
        
        return dtos.ToPagedQueryResult(request.Filter, rowCount);
    }
}