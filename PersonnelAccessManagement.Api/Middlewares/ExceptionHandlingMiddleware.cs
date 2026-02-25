using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PersonnelAccessManagement.Api.Observability;
using PersonnelAccessManagement.Application.Common.Exceptions;
using Serilog;

namespace PersonnelAccessManagement.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
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
            if (ctx.Response.HasStarted)
            {
                Log.Warning(ex, "Response already started. path={Path}", ctx.Request.Path);
                throw;
            }
            
            var header = ObservabilityConstants.CorrelationHeader;
            var cid = ctx.Items[header] as string;

            if (string.IsNullOrWhiteSpace(cid))
                cid = ctx.Request.Headers[header].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(cid))
                cid = ctx.TraceIdentifier;
            
            if (ex is OperationCanceledException && ctx.RequestAborted.IsCancellationRequested)
            {
                Log.Information("Request cancelled by client. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);
                return;
            }

            ctx.Response.ContentType = "application/problem+json";

            if (ex is ValidationException vex)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var pd = new ValidationProblemDetails(errors)
                {
                    Status = ctx.Response.StatusCode,
                    Title = "Validation failed.",
                    Type = "https://httpstatuses.com/400",
                    Instance = ctx.Request.Path
                };

                pd.Extensions["code"] = "validation_error";
                pd.Extensions["correlationId"] = cid;

                Log.Warning(ex, "Validation error. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);

                await ctx.Response.WriteAsJsonAsync(pd, cancellationToken: ctx.RequestAborted);
                return;
            }

            if (ex is AppException appEx)
            {
                ctx.Response.StatusCode = appEx.StatusCode;

                var pd = new ProblemDetails
                {
                    Status = appEx.StatusCode,
                    Title = appEx.Message,
                    Type = $"https://httpstatuses.com/{appEx.StatusCode}",
                    Instance = ctx.Request.Path
                };

                pd.Extensions["code"] = appEx.Code;
                pd.Extensions["correlationId"] = cid;

                Log.Warning(ex, "Application exception. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);

                await ctx.Response.WriteAsJsonAsync(pd, cancellationToken: ctx.RequestAborted);
                return;
            }

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var unknown = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected server error.",
                Type = "https://httpstatuses.com/500",
                Instance = ctx.Request.Path
            };

            unknown.Extensions["code"] = "server_error";
            unknown.Extensions["correlationId"] = cid;

            Log.Error(ex, "Unhandled exception. trace.id={TraceId} path={Path}", cid, ctx.Request.Path);

            await ctx.Response.WriteAsJsonAsync(unknown, cancellationToken: ctx.RequestAborted);
        }
    }
}