using PersonnelAccessManagement.Application.Common.Interfaces;

namespace PersonnelAccessManagement.Infrastructure.Auth;

public sealed class SimplePasswordVerifier : IPasswordVerifier
{
    private const string DevPassword = "Pam2026!";

    public Task<bool> VerifyAsync(string employeeNo, string password, CancellationToken ct)
    {
        var isValid = password == DevPassword;
        return Task.FromResult(isValid);
    }
}