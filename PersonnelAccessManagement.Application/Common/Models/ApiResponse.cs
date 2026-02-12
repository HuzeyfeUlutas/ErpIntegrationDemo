namespace PersonnelAccessManagement.Application.Common.Models;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    ApiError? Error,
    string? CorrelationId);
    
public sealed record ApiError(
    string Code,
    string Message,
    IDictionary<string, string[]>? Details = null);