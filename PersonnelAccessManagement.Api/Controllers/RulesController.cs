using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.Rules.Commands.CreateRule;
using PersonnelAccessManagement.Application.Features.Rules.Commands.DeleteRule;
using PersonnelAccessManagement.Application.Features.Rules.Commands.UpdateRule;
using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using PersonnelAccessManagement.Application.Features.Rules.Queries;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/rules")]
[Authorize(Roles = "Admin")]
public sealed class RulesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RulesController(IMediator mediator) => _mediator = mediator;
    
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] RuleFilter filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListRulesQuery(filter), ct);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRuleCommand cmd, CancellationToken ct)
    {
        var id = await _mediator.Send(cmd, ct);
        
        return Created($"/api/rules/{id}", new { id });
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRuleCommand cmd, CancellationToken ct)
    {
        await _mediator.Send(cmd, ct);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRuleCommand(id), ct);
        return NoContent();
    }
}