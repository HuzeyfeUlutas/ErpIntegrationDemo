using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Application.Features.Roles.Queries;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator) => _mediator = mediator;

    // GET /api/roles?PageIndex=1&PageSize=100&Name=...
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] RoleFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListRolesQuery(filter), ct);
        return Ok(result);
    }
}