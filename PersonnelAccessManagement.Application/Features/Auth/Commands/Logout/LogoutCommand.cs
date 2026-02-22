using MediatR;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest;