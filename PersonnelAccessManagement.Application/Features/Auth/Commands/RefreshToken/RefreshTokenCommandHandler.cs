using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Options;
using PersonnelAccessManagement.Application.Features.Auth.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRepository<Domain.Entities.RefreshToken> _refreshTokenRepo;
    private readonly IRepository<Personnel> _personnelRepo;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _uow;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IRepository<Domain.Entities.RefreshToken> refreshTokenRepo,
        IRepository<Personnel> personnelRepo,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork uow,
        IOptions<JwtOptions> jwtOptions,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _refreshTokenRepo = refreshTokenRepo;
        _personnelRepo = personnelRepo;
        _tokenGenerator = tokenGenerator;
        _uow = uow;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var existing = await _refreshTokenRepo.Query()
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, ct);

        if (existing is null || !existing.IsActive)
        {
            _logger.LogWarning("Refresh token invalid or expired.");
            throw new UnauthorizedException("Geçersiz veya süresi dolmuş refresh token.");
        }
        
        var personnel = await _personnelRepo.QueryAsNoTracking()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.EmployeeNo == decimal.Parse(existing.EmployeeNo), ct);

        if (personnel is null || personnel.IsDeleted)
        {
            existing.Revoke();
            await _uow.SaveChangesAsync(ct);
            throw new UnauthorizedException("Kullanıcı bulunamadı.");
        }
        
        existing.Revoke();
        
        var newRefreshToken = new Domain.Entities.RefreshToken(
            existing.EmployeeNo, _jwtOptions.RefreshTokenExpirationDays);
        await _refreshTokenRepo.AddAsync(newRefreshToken, ct);
        await _uow.SaveChangesAsync(ct);
        
        var accessToken = _tokenGenerator.GenerateToken(personnel);

        _logger.LogInformation("Token refreshed — EmployeeNo: {EmployeeNo}", existing.EmployeeNo);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken.Token,
            EmployeeNo: existing.EmployeeNo,
            FullName: personnel.FullName,
            AccessTokenExpiresAtUtc: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
        );
    }
}