namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IPasswordVerifier
{
    Task<bool> VerifyAsync(string employeeNo, string password, CancellationToken ct);
}