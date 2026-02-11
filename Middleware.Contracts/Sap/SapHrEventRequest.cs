using System.Text.Json.Serialization;

namespace Middleware.Contracts.Sap;

public sealed record SapHrEventRequest(
    [property: JsonPropertyName("eventType")] int EventType,
    [property: JsonPropertyName("employeeNo")] string EmployeeNo,
    [property: JsonPropertyName("effectiveDate")] DateOnly EffectiveDate
);