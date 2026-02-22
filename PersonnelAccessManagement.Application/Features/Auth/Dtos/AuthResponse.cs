namespace PersonnelAccessManagement.Application.Features.Auth.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string EmployeeNo,
    string FullName,
    DateTime AccessTokenExpiresAtUtc
);