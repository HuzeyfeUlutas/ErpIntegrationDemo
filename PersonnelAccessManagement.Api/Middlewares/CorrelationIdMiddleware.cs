using PersonnelAccessManagement.Api.Observability;
using Serilog.Context;

namespace PersonnelAccessManagement.Api.Middlewares;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        var header = ObservabilityConstants.CorrelationHeader;

        var cid = ctx.Request.Headers[header].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(cid))
            cid = Guid.NewGuid().ToString();

        ctx.Items[header] = cid;
        ctx.Response.Headers[header] = cid;

        using (LogContext.PushProperty(ObservabilityConstants.TraceIdProperty, cid))
        {
            await _next(ctx);
        }
    }
}