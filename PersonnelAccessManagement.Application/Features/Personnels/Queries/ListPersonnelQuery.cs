using MediatR;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Personnels.Dtos;

namespace PersonnelAccessManagement.Application.Features.Personnels.Queries;

public sealed record ListPersonnelQuery(PersonnelFilter Filter) : IRequest<PagedQueryResult<IEnumerable<PersonnelDto>>>;