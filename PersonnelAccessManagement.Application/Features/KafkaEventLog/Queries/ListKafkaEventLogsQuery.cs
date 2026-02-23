using MediatR;
using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Application.Features.KafkaEventLog.Dtos;

namespace PersonnelAccessManagement.Application.Features.KafkaEventLog.Queries;

public sealed record ListKafkaEventLogsQuery(KafkaEventLogFilter Filter)
    : IRequest<PagedQueryResult<IEnumerable<KafkaEventLogDto>>>;