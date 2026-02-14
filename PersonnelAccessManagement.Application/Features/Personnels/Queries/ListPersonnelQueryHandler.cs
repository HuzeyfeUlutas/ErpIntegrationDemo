using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Personnels.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Personnels.Queries;

public sealed class ListPersonnelQueryHandler 
    : IRequestHandler<ListPersonnelQuery, PagedQueryResult<IEnumerable<PersonnelDto>>>
{
    private readonly IRepository<Personnel> _personnels;
    private readonly IMapper _mapper;

    public ListPersonnelQueryHandler(IRepository<Personnel> personnels, IMapper mapper)
    {
        _personnels = personnels;
        _mapper = mapper;
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

        var dtos = _mapper.Map<List<PersonnelDto>>(entities);

        return dtos.ToPagedQueryResult(request.Filter, rowCount);
    }
}