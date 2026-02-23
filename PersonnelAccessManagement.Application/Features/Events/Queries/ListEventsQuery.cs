using MediatR;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.Events.Dtos;

namespace PersonnelAccessManagement.Application.Features.Events.Queries;

public sealed record ListEventsQuery(EventFilter Filter)
    : IRequest<PagedQueryResult<IEnumerable<EventDto>>>;