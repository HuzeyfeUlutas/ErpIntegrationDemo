using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Extensions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Infrastructure.Services;

public sealed class PersonnelRoleService : IPersonnelRoleService
{
    private const int BatchSize = 200;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IPersonnelRoleBatchProcessor _batchProcessor;
    private readonly ILogger<PersonnelRoleService> _logger;

    public PersonnelRoleService(
        IServiceScopeFactory scopeFactory,
        IPersonnelRoleBatchProcessor batchProcessor,
        ILogger<PersonnelRoleService> logger)
    {
        _scopeFactory = scopeFactory;
        _batchProcessor = batchProcessor;
        _logger = logger;
    }

    public async Task ApplyCreatedRuleToMatchingPersonnelAsync(
        Guid ruleId, string correlationId, CancellationToken ct = default)
    {
        // 1) Setup — kendi scope'u
        var (eventId, roleIds, campus, title) = await SetupAsync(ruleId, correlationId, ct);

        // 2) Batch loop
        var lastEmployeeNo = 0m;
        int totalSuccess = 0, totalFail = 0, processed = 0;

        while (true)
        {
            // Personel ID'lerini hafif bir query ile çek (kendi scope'u)
            List<Guid> personnelIds;
            decimal batchLastNo;

            using (var scope = _scopeFactory.CreateScope())
            {
                var personnelRepo = scope.ServiceProvider
                    .GetRequiredService<IRepository<Personnel>>();

                var batch = await personnelRepo.QueryAsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .FilterIf(p => p.Campus == campus, campus != null)
                    .FilterIf(p => p.Title == title, title != null)
                    .Where(p => p.EmployeeNo > lastEmployeeNo)
                    .OrderBy(p => p.EmployeeNo)
                    .Take(BatchSize)
                    .Select(p => new { p.Id, p.EmployeeNo })
                    .ToListAsync(ct);

                if (batch.Count == 0) break;

                personnelIds = batch.Select(b => b.Id).ToList();
                batchLastNo = batch[^1].EmployeeNo;
            }

            // Generic processor'a gönder
            var result = await _batchProcessor.ProcessBatchAsync(
                personnelIds,
                roleIdsToAdd: roleIds,
                roleIdsToRemove: new List<decimal>(),
                eventId,
                ct);

            totalSuccess += result.SuccessCount;
            totalFail += result.FailCount;
            processed += personnelIds.Count;
            lastEmployeeNo = batchLastNo;

            _logger.LogInformation("Rule {RuleId}: {Processed} processed", ruleId, processed);
        }

        // 3) Complete
        await CompleteAsync(eventId, processed, totalSuccess, totalFail, ct);

        _logger.LogInformation(
            "Rule {RuleId} done — Total: {Total}, Success: {Success}, Fail: {Fail}",
            ruleId, processed, totalSuccess, totalFail);
    }

    public Task ApplyUpdatedRuleToMatchingPersonnelAsync(Guid ruleId, string correlationId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task ApplyDeletedRuleToMatchingPersonnelAsync(Guid ruleId, string correlationId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    private async Task<(Guid EventId, List<decimal> RoleIds, Campus? Campus, Title? Title)>
        SetupAsync(Guid ruleId, string correlationId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var ruleRepo = sp.GetRequiredService<IRepository<Rule>>();
        var eventRepo = sp.GetRequiredService<IRepository<Event>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        var rule = await ruleRepo.QueryAsNoTracking()
                       .FirstOrDefaultAsync(r => r.Id == ruleId && !r.IsDeleted, ct)
                   ?? throw new NotFoundException($"Rule {ruleId} not found.");

        var roleIds = await ruleRepo.QueryAsNoTracking()
            .Where(r => r.Id == ruleId)
            .SelectMany(r => r.Roles)
            .Select(r => r.Id)
            .ToListAsync(ct);

        var sourceDetail = JsonSerializer.Serialize(new
        {
            campus = rule.Campus, title = rule.Title, roleIds
        });

        var evt = new Event(EventType.RuleCreated, ruleId.ToString(), correlationId, sourceDetail);
        await eventRepo.AddAsync(evt, ct);
        await uow.SaveChangesAsync(ct);

        return (evt.Id, roleIds, rule.Campus, rule.Title);
    }

    private async Task CompleteAsync(
        Guid eventId, int total, int success, int fail, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var eventRepo = sp.GetRequiredService<IRepository<Event>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        var evt = await eventRepo.Query().FirstAsync(e => e.Id == eventId, ct);
        evt.Complete(total, success, fail);
        await uow.SaveChangesAsync(ct);
    }
}