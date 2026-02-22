namespace PersonnelAccessManagement.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;
    public string EmployeeNo { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Revoke edildiğinde doldurulur (logout, şifre değişikliği vb.)
    /// </summary>
    public DateTime? RevokedAtUtc { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken() { }

    public RefreshToken(string employeeNo, int expirationDays)
    {
        Id = Guid.NewGuid();
        Token = GenerateToken();
        EmployeeNo = employeeNo;
        ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (!IsRevoked)
            RevokedAtUtc = DateTime.UtcNow;
    }

    private static string GenerateToken()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}