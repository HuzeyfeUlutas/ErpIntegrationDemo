using Dapper;
using Npgsql;

namespace Middleware.Infrastructure.Persistence;

public sealed class DbInitializer(DbOptions opt)
{
    private readonly string _cs = opt.ConnectionString;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        const string sql = """
                           create table if not exists processed_requests(
                             request_id text primary key,
                             processed_at_utc timestamptz not null
                           );
                           """;
        await using var conn = new NpgsqlConnection(_cs);
        await conn.ExecuteAsync(new CommandDefinition(sql, cancellationToken: ct));
    }
}