using MediatR;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Rules.Dtos;
namespace PersonnelAccessManagement.Application.Features.Rules.Queries;


public sealed record ListRulesQuery(RuleFilter Filter) : IRequest<PagedQueryResult<IEnumerable<RuleDto>>>;