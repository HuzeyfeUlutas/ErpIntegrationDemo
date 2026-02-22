using System.Security.Claims;
using PersonnelAccessManagement.Application.Common.Interfaces;

namespace PersonnelAccessManagement.Api.Services;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? EmployeeNo =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("employeeNo");

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}