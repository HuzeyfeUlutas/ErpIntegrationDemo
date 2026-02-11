using Dapper;
using Npgsql;
using MiddlewareApplication.Abstractions;

namespace Middleware.Infrastructure.Persistence;

public sealed class ProcessedRequestsRepository(DbOptions opt) : IProcessedRequestsStore
{
    private readonly string _cs = opt.ConnectionString;

    public async Task<bool> IsProcessedAsync(string requestId, CancellationToken ct)
    {
        const string sql = "select 1 from processed_requests where request_id = @requestId limit 1;";
        await using var conn = new NpgsqlConnection(_cs);
        var res = await conn.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { requestId }, cancellationToken: ct));
        return res.HasValue;
    }

    public async Task MarkProcessedAsync(string requestId, CancellationToken ct)
    {
        const string sql = """
                           insert into processed_requests (request_id, processed_at_utc)
                           values (@requestId, now())
                           on conflict (request_id) do nothing;
                           """;
        await using var conn = new NpgsqlConnection(_cs);
        await conn.ExecuteAsync(new CommandDefinition(sql, new { requestId }, cancellationToken: ct));
    }
}