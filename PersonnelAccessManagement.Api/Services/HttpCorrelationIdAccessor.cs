using PersonnelAccessManagement.Api.Observability;
using PersonnelAccessManagement.Application.Common.Interfaces;

namespace PersonnelAccessManagement.Api.Services;

public sealed class HttpCorrelationIdAccessor : ICorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId
    {
        get
        {
            var ctx = _httpContextAccessor.HttpContext;
            var cid = ctx?.Items[ObservabilityConstants.CorrelationHeader] as string;
            return cid ?? Guid.NewGuid().ToString("N");
        }
    }
}