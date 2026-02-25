using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Middleware.Contracts.Sap;
using Middleware.Infrastructure.Security;
using MiddlewareApplication.Ingestion;

namespace Middleware.Api.Controllers;

[ApiController]
[Route("sap/hr/events")]
public sealed class SapEventsController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    [HttpPost]
    public async Task<IActionResult> Receive(
        [FromServices] HmacSignatureVerifier verifier,
        [FromServices] SapEventIngestionHandler handler,
        CancellationToken ct)
    {
        var requestId = Request.Headers["X-Request-Id"].ToString();
        var timestamp = Request.Headers["X-Timestamp"].ToString();
        var signature = Request.Headers["X-Signature"].ToString();
        var correlationId = Request.Headers["X-Correlation-Id"].ToString();
        if (string.IsNullOrWhiteSpace(correlationId)) correlationId = requestId;

        if (string.IsNullOrWhiteSpace(requestId)
            || string.IsNullOrWhiteSpace(timestamp)
            || string.IsNullOrWhiteSpace(signature))
            return Unauthorized("Missing signature headers.");

        // Read raw body
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        if (!verifier.Verify(body, timestamp, signature))
            return Unauthorized("Invalid signature.");

        SapHrEventRequest? req;
        try
        {
            req = JsonSerializer.Deserialize<SapHrEventRequest>(body, JsonOpts);
        }
        catch (JsonException)
        {
            return BadRequest("Invalid JSON.");
        }

        if (req is null) return BadRequest("Invalid JSON.");

        var result = await handler.HandleAsync(req, requestId, correlationId, ct);

        return result.Status switch
        {
            "accepted" => Accepted(new
            {
                result.RequestId,
                result.CorrelationId,
                result.EventId
            }),
            "deduplicated" => Ok(new
            {
                result.RequestId,
                result.CorrelationId,
                message = "Already processed"
            }),
            "rejected" => BadRequest(new
            {
                result.Reason
            }),
            _ => StatusCode(500)
        };
    }
}