namespace PersonnelAccessManagement.Application.Common.Interfaces;

/// <summary>
/// Şifre doğrulama servisi.
/// LDAP, Active Directory veya DB tabanlı implementasyon yapılabilir.
/// </summary>
public interface IPasswordVerifier
{
    Task<bool> VerifyAsync(string employeeNo, string password, CancellationToken ct);
}