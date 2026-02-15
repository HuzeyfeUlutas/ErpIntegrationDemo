using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.Personnels.Commands.UpdatePersonnel;
using PersonnelAccessManagement.Application.Features.Personnels.Dtos;
using PersonnelAccessManagement.Application.Features.Personnels.Queries;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/personnels")]
public sealed class PersonnelsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PersonnelsController(IMediator mediator) => _mediator = mediator;

    // GET /api/personnels?PageIndex=1&PageSize=10&Search=...&Campus=...&Title=...
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PersonnelFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListPersonnelQuery(filter), ct);
        return Ok(result);
    }

    // PUT /api/personnels
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePersonnelCommand cmd, CancellationToken ct)
    {
        await _mediator.Send(cmd, ct);
        return NoContent();
    }
}