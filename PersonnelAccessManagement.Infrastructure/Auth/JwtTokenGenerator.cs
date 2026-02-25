using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Options;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Infrastructure.Auth;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly AdminSettings _adminSettings;

    public JwtTokenGenerator(
        IOptions<JwtOptions> jwtOptions,
        IOptions<AdminSettings> adminSettings)
    {
        _jwtOptions = jwtOptions.Value;
        _adminSettings = adminSettings.Value;
    }

    public string GenerateToken(Personnel personnel)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var employeeNo = personnel.EmployeeNo.ToString("F0");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, employeeNo),
            new("employeeNo", employeeNo),
            new(ClaimTypes.Name, personnel.FullName),
            new("campus", personnel.Campus.ToString()),
            new("title", personnel.Title.ToString()),
        };
        
        if (_adminSettings.AdminEmployeeNos.Contains(employeeNo))
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}