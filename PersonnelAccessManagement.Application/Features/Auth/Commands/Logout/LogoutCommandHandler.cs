using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Interfaces;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRepository<Domain.Entities.RefreshToken> _refreshTokenRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRepository<Domain.Entities.RefreshToken> refreshTokenRepo,
        IUnitOfWork uow,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepo = refreshTokenRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var token = await _refreshTokenRepo.Query()
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, ct);

        if (token is null) return;
        
        var allTokens = await _refreshTokenRepo.Query()
            .Where(x => x.EmployeeNo == token.EmployeeNo && x.RevokedAtUtc == null)
            .ToListAsync(ct);

        foreach (var t in allTokens)
            t.Revoke();

        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Logout — EmployeeNo: {EmployeeNo}, {Count} token(s) revoked.",
            token.EmployeeNo, allTokens.Count);
    }
}