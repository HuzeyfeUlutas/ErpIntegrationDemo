using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Infrastructure.Jobs;

/// <summary>
/// Hangfire recurring job — Her gün 09:00'da çalışır.
/// 
/// 3 izole scope:
///   1) İş mantığı (process + action status)
///   2) Job log (DB loglama)
///   3) Finalize (job status)
/// </summary>
public sealed class DailyScheduledActionJob : IScheduledActionJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyScheduledActionJob> _logger;

    public DailyScheduledActionJob(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyScheduledActionJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var tomorrow = today.AddDays(1);

        _logger.LogInformation("DailyScheduledActionJob started — Date: {Date}", today);

        // ── 1) Job oluştur + action ID'leri çek ─────────────────
        Guid jobId;
        List<long> actionIds;

        using (var scope = _scopeFactory.CreateScope())
        {
            var actionRepo = scope.ServiceProvider.GetRequiredService<IRepository<PersonnelScheduledAction>>();
            var jobRepo = scope.ServiceProvider.GetRequiredService<IRepository<Job>>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            actionIds = await actionRepo.QueryAsNoTracking()
                .Where(x => x.Status == ScheduledActionStatus.Pending)
                .Where(x =>
                    (x.ActionType == ScheduledActionType.Hire && x.EffectiveDate <= today) ||
                    (x.ActionType == ScheduledActionType.Terminate && x.EffectiveDate <= tomorrow))
                .OrderBy(x => x.EffectiveDate)
                .ThenBy(x => x.Id)
                .Select(x => x.Id)
                .ToListAsync(ct);

            var job = new Job(JobType.DailyScheduledAction);
            job.SetTotal(actionIds.Count);
            await jobRepo.AddAsync(job, ct);
            await uow.SaveChangesAsync(ct);
            jobId = job.Id;
        }

        if (actionIds.Count == 0)
        {
            _logger.LogInformation("No pending actions for {Date}", today);
            await FinalizeJob(jobId, ct);
            return;
        }

        _logger.LogInformation("Found {Count} pending action(s)", actionIds.Count);

        // ── 2) Her action'ı işle ────────────────────────────────
        foreach (var actionId in actionIds)
        {
            var result = await ProcessAction(actionId, ct);
            await WriteJobLog(jobId, result, ct);
        }

        // ── 3) Job'u tamamla ────────────────────────────────────
        await FinalizeJob(jobId, ct);
    }

    /// <summary>
    /// Scope 1: İş mantığı + action status güncelleme.
    /// </summary>
    private async Task<ActionResult> ProcessAction(long actionId, CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var actionRepo = scope.ServiceProvider.GetRequiredService<IRepository<PersonnelScheduledAction>>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var processor = scope.ServiceProvider.GetRequiredService<IScheduledActionProcessor>();

            var action = await actionRepo.Query().FirstOrDefaultAsync(x => x.Id == actionId, ct);
            if (action is null)
                return ActionResult.Fail(actionId, "Action not found");

            switch (action.ActionType)
            {
                case ScheduledActionType.Hire:
                    await processor.ProcessHireAsync(action, ct);
                    break;
                case ScheduledActionType.Terminate:
                    await processor.ProcessTerminationAsync(action, ct);
                    break;
                default:
                    return ActionResult.Fail(actionId, $"Unknown ActionType: {action.ActionType}");
            }

            action.Status = ScheduledActionStatus.Completed;
            action.ProcessedAtUtc = DateTime.UtcNow;
            await uow.SaveChangesAsync(ct);

            _logger.LogInformation("Action {Id} completed — {ActionType} for {EmployeeNo}",
                action.Id, action.ActionType, action.EmployeeNo);

            return ActionResult.Ok(actionId, $"{action.ActionType} completed for EmployeeNo: {action.EmployeeNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Action {Id} failed. Will retry next run.", actionId);
            return ActionResult.Fail(actionId, ex.Message);
        }
    }

    /// <summary>
    /// Scope 2: Job log yazma — Job entity'sine dokunmadan doğrudan JobLog ekler.
    /// </summary>
    private async Task WriteJobLog(Guid jobId, ActionResult result, CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var logRepo = scope.ServiceProvider.GetRequiredService<IRepository<JobLog>>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var status = result.Success ? "SUCCESS" : "FAILED";
            var log = new JobLog(jobId, result.Message, status);

            await logRepo.AddAsync(log, ct);
            await uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to write job log for action {ActionId}", result.ActionId);
        }
    }

    /// <summary>
    /// Scope 3: Job status finalize — loglardan count hesaplar.
    /// </summary>
    private async Task FinalizeJob(Guid jobId, CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var jobRepo = scope.ServiceProvider.GetRequiredService<IRepository<Job>>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IRepository<JobLog>>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var job = await jobRepo.Query().FirstAsync(x => x.Id == jobId, ct);

            var successCount = await logRepo.QueryAsNoTracking()
                .CountAsync(x => x.JobId == jobId && x.Status == "SUCCESS", ct);
            var failureCount = await logRepo.QueryAsNoTracking()
                .CountAsync(x => x.JobId == jobId && x.Status == "FAILED", ct);

            job.Finalize(successCount, failureCount);
            await uow.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Job finalized — JobId: {JobId}, Status: {Status}, Total: {Total}, Success: {Success}, Failed: {Failed}",
                job.Id, job.Status, job.TotalCount, successCount, failureCount);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to finalize job {JobId}", jobId);
        }
    }

    /// <summary>
    /// Scope'lar arası veri taşımak için basit DTO.
    /// </summary>
    private sealed record ActionResult(long ActionId, bool Success, string Message)
    {
        public static ActionResult Ok(long actionId, string message) => new(actionId, true, message);
        public static ActionResult Fail(long actionId, string message) => new(actionId, false, message);
    }
}