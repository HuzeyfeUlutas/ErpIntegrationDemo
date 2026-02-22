namespace PersonnelAccessManagement.Application.Common.Interfaces;

/// <summary>
/// Authorize olan kullanıcının bilgilerini JWT token'dan okur.
/// </summary>
public interface ICurrentUserAccessor
{
    string? EmployeeNo { get; }
    bool IsAuthenticated { get; }
}