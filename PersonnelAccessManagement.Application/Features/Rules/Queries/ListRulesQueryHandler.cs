using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using Rule = PersonnelAccessManagement.Domain.Entities.Rule;

namespace PersonnelAccessManagement.Application.Features.Rules.Queries;

public sealed class ListRulesQueryHandler 
    : IRequestHandler<ListRulesQuery, PagedQueryResult<IEnumerable<RuleDto>>>
{
    private readonly IRepository<Rule> _rules;
    private readonly IMapper _mapper;

    public ListRulesQueryHandler(IRepository<Rule> rules, IMapper mapper)
    {
        _rules = rules;
        _mapper = mapper;
    }

    public async Task<PagedQueryResult<IEnumerable<RuleDto>>> Handle(ListRulesQuery request, CancellationToken ct)
    {
        var name = request.Filter.Name?.Trim();
        
        var q = _rules.QueryAsNoTracking()
            .Include(r => r.Roles) // Eğer RoleIds mapliyorsan şart değil
            .Where(r => !r.IsDeleted)
            .FilterIf(r => EF.Functions.Like(r.Name, $"{name}%"), !string.IsNullOrWhiteSpace(name))
            .FilterIf(r => r.Campus == request.Filter.Campus, request.Filter.Campus is not null)
            .FilterIf(r => r.Title == request.Filter.Title, request.Filter.Title is not null);
        
        // 🔹 1) Toplam kayıt
        var rowCount = await q.CountAsync(ct);

        // 🔹 2) Paging
        var pagedQuery = q
            .OrderByDescending(r => r.CreatedAt)
            .GetPaged(request.Filter);

        var entities = await pagedQuery.ToListAsync(ct);

        // 🔹 3) Mapping
        var dtos = _mapper.Map<List<RuleDto>>(entities);

        // 🔹 4) Paged result
        return dtos.ToPagedQueryResult(request.Filter, rowCount);
    }
}