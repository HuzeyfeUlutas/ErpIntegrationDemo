using MediatR;
using PersonnelAccessManagement.Application.Features.Events.Dtos;

namespace PersonnelAccessManagement.Application.Features.Events.Queries;

public sealed record GetEventLogsQuery(Guid EventId) : IRequest<List<EventLogDto>>;
