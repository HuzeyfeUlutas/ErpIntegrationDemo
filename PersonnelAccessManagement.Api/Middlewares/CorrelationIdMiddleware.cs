using Serilog.Context;

namespace PersonnelAccessManagement.Api.Middlewares;

public sealed class CorrelationIdMiddleware
{
    private const string Header = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        var cid = ctx.Request.Headers[Header].ToString();
        if (string.IsNullOrWhiteSpace(cid))
            cid = Guid.NewGuid().ToString();

        ctx.Items[Header] = cid;
        ctx.Response.Headers[Header] = cid;
        
        using (LogContext.PushProperty("trace.id", cid))
        {
            await _next(ctx);
        }
    }
}