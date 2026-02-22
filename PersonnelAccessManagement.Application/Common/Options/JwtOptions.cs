using System.ComponentModel.DataAnnotations;

namespace PersonnelAccessManagement.Application.Common.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Secret { get; init; } = default!;

    [Required]
    public string Issuer { get; init; } = default!;

    [Required]
    public string Audience { get; init; } = default!;

    /// <summary>
    /// Access token süresi (dakika). Kısa tutulmalı.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; init; } = 60;

    /// <summary>
    /// Refresh token süresi (gün).
    /// </summary>
    public int RefreshTokenExpirationDays { get; init; } = 7;
}