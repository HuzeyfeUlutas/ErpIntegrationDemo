using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.KafkaEventLog.Dtos;
using PersonnelAccessManagement.Application.Features.KafkaEventLog.Queries;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/kafka-events")]
[Authorize(Roles = "Admin")]
public sealed class KafkaEventLogsController : ControllerBase
{
    private readonly IMediator _mediator;
    public KafkaEventLogsController(IMediator mediator) => _mediator = mediator;

    // GET /api/kafka-events?PageIndex=1&PageSize=20&Search=...&Status=...&EventType=...
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] KafkaEventLogFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListKafkaEventLogsQuery(filter), ct);
        return Ok(result);
    }
}