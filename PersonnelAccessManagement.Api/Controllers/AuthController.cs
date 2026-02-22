using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Application.Features.Auth.Commands;
using PersonnelAccessManagement.Application.Features.Auth.Commands.Logout;
using PersonnelAccessManagement.Application.Features.Auth.Commands.RefreshToken;

namespace PersonnelAccessManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// EmployeeNo + Password ile giriş yap, Access + Refresh token al.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    /// <summary>
    /// Refresh token ile yeni Access + Refresh token al.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    /// <summary>
    /// Çıkış yap — tüm refresh token'ları revoke et.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand cmd, CancellationToken ct)
    {
        await _mediator.Send(cmd, ct);
        return NoContent();
    }
}