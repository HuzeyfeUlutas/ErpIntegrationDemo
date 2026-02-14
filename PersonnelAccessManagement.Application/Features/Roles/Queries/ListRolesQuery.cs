using MediatR;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Roles.Dtos;

namespace PersonnelAccessManagement.Application.Features.Roles.Queries;

public sealed record ListRolesQuery(RoleFilter Filter) : IRequest<PagedQueryResult<IEnumerable<RoleDto>>>;