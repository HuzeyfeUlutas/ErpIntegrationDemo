using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Models;
using Serilog;
using ValidationException = FluentValidation.ValidationException;

namespace PersonnelAccessManagement.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private const string Header = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            // Response başladıysa burada JSON yazmaya çalışma
            if (ctx.Response.HasStarted)
            {
                Log.Warning(ex, "Response already started. path={Path}", ctx.Request.Path);
                throw;
            }

            var cid =
                ctx.Items[Header]?.ToString()
                ?? ctx.Request.Headers[Header].ToString()
                ?? ctx.Response.Headers[Header].ToString();

            // Client iptal ettiyse error loglama
            if (ex is OperationCanceledException && ctx.RequestAborted.IsCancellationRequested)
            {
                Log.Information("Request cancelled by client. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);
                ctx.Response.StatusCode = 499; // opsiyonel
                return;
            }

            ctx.Response.ContentType = "application/json";

            if (ex is ValidationException vex)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;

                var details = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                Log.Warning(ex, "Validation error. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);

                await ctx.Response.WriteAsJsonAsync(
                    new ApiResponse<object>(
                        false,
                        null,
                        new ApiError("validation_error", "Validation failed.", details),
                        cid));

                return;
            }

            if (ex is AppException appEx)
            {
                ctx.Response.StatusCode = appEx.StatusCode;

                Log.Warning(ex, "Application exception. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);

                await ctx.Response.WriteAsJsonAsync(
                    new ApiResponse<object>(
                        false,
                        null,
                        new ApiError(appEx.Code, appEx.Message),
                        cid));

                return;
            }

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

            Log.Error(ex, "Unhandled exception. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);

            await ctx.Response.WriteAsJsonAsync(
                new ApiResponse<object>(
                    false,
                    null,
                    new ApiError("server_error", "Unexpected server error."),
                    cid));
        }
    }
}