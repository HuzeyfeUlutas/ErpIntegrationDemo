using MediatR;
using PersonnelAccessManagement.Application.Features.Auth.Dtos;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string EmployeeNo,
    string Password
) : IRequest<AuthResponse>;

