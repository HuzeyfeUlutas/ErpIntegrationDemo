using System.Text.Json.Serialization;
using Middleware.Contracts.Enums;

namespace Middleware.Contracts.Sap;

public sealed record SapHrEventRequest(
    [property: JsonPropertyName("eventType")] PersonnelEventType EventType,
    [property: JsonPropertyName("employeeNo")] string EmployeeNo,
    [property: JsonPropertyName("effectiveDate")] DateOnly EffectiveDate
);