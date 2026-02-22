using MediatR;
using PersonnelAccessManagement.Application.Features.Auth.Dtos;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;