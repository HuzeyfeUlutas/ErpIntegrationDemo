using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Application.Common.Options;
using PersonnelAccessManagement.Application.Features.Auth.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IRepository<Personnel> _personnelRepo;
    private readonly IRepository<Domain.Entities.RefreshToken> _refreshTokenRepo;
    private readonly IPasswordVerifier _passwordVerifier;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _uow;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IRepository<Personnel> personnelRepo,
        IRepository<Domain.Entities.RefreshToken> refreshTokenRepo,
        IPasswordVerifier passwordVerifier,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork uow,
        IOptions<JwtOptions> jwtOptions,
        ILogger<LoginCommandHandler> logger)
    {
        _personnelRepo = personnelRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _passwordVerifier = passwordVerifier;
        _tokenGenerator = tokenGenerator;
        _uow = uow;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        // 1) Personeli bul
        var personnel = await _personnelRepo.QueryAsNoTracking()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.EmployeeNo == decimal.Parse(request.EmployeeNo), ct);

        if (personnel is null)
        {
            _logger.LogWarning("Login failed — EmployeeNo {EmployeeNo} not found.", request.EmployeeNo);
            throw new UnauthorizedException("Geçersiz kullanıcı adı veya şifre.");
        }

        if (personnel.IsDeleted)
        {
            _logger.LogWarning("Login failed — EmployeeNo {EmployeeNo} is soft-deleted.", request.EmployeeNo);
            throw new UnauthorizedException("Geçersiz kullanıcı adı veya şifre.");
        }

        // 2) Şifre doğrula
        var valid = await _passwordVerifier.VerifyAsync(request.EmployeeNo, request.Password, ct);
        if (!valid)
        {
            _logger.LogWarning("Login failed — invalid password for {EmployeeNo}.", request.EmployeeNo);
            throw new UnauthorizedException("Geçersiz kullanıcı adı veya şifre.");
        }

        // 3) Eski refresh token'ları revoke et
        var existingTokens = await _refreshTokenRepo.Query()
            .Where(x => x.EmployeeNo == request.EmployeeNo && x.RevokedAtUtc == null)
            .ToListAsync(ct);

        foreach (var old in existingTokens)
            old.Revoke();

        // 4) Yeni refresh token oluştur
        var refreshToken = new Domain.Entities.RefreshToken(request.EmployeeNo, _jwtOptions.RefreshTokenExpirationDays);
        await _refreshTokenRepo.AddAsync(refreshToken, ct);
        await _uow.SaveChangesAsync(ct);

        // 5) Access token üret
        var accessToken = _tokenGenerator.GenerateToken(personnel);

        _logger.LogInformation("Login successful — EmployeeNo: {EmployeeNo}", request.EmployeeNo);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token,
            EmployeeNo: request.EmployeeNo,
            FullName: personnel.FullName,
            AccessTokenExpiresAtUtc: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
        );
    }
}