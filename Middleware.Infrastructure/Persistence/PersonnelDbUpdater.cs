using Dapper;
using Microsoft.Extensions.Logging;
using MiddlewareApplication.Abstractions;
using Npgsql;

namespace Middleware.Infrastructure.Persistence;

public sealed class PersonnelDbUpdater : IPersonnelDbUpdater
{
    private readonly PamDbOptions _pamDbOptions;
    private readonly ILogger<PersonnelDbUpdater> _logger;

    public PersonnelDbUpdater(PamDbOptions pamDbOptions, ILogger<PersonnelDbUpdater> logger)
    {
        _pamDbOptions = pamDbOptions;
        _logger = logger;
    }

    public async Task<PositionUpdateResult> UpdatePositionAsync(string employeeNo, CancellationToken ct)
    {
        try
        {
            if (!decimal.TryParse(employeeNo, out var empNo))
            {
                _logger.LogError("Invalid EmployeeNo format: {EmployeeNo}", employeeNo);
                return new PositionUpdateResult(false, null, null, null, null, $"Invalid EmployeeNo: {employeeNo}");
            }

            const string sql = "SELECT * FROM usp_update_personnel_position(@p_employee_no);";

            await using var conn = new NpgsqlConnection(_pamDbOptions.ConnectionString);
            var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                new CommandDefinition(sql, new { p_employee_no = empNo }, cancellationToken: ct));

            if (result is null)
            {
                _logger.LogWarning("SP returned no result for EmployeeNo: {EmployeeNo}", employeeNo);
                return new PositionUpdateResult(false, null, null, null, null, "Personnel not found");
            }

            _logger.LogInformation(
                "Position updated via SP — EmployeeNo: {EmployeeNo}, " +
                "({OldCampus},{OldTitle}) → ({NewCampus},{NewTitle})",
                employeeNo, result.old_campus, result.old_title,
                result.new_campus, result.new_title);

            return new PositionUpdateResult(
                true,
                result.old_campus,
                result.old_title,
                result.new_campus,
                result.new_title,
                null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SP call failed for EmployeeNo: {EmployeeNo}", employeeNo);
            return new PositionUpdateResult(false, null, null, null, null, ex.Message);
        }
    }

    private sealed class SpResult
    {
        public decimal employee_no { get; init; }
        public string? old_campus { get; init; }
        public string? old_title { get; init; }
        public string? new_campus { get; init; }
        public string? new_title { get; init; }
        public int rows_affected { get; init; }
    }
}