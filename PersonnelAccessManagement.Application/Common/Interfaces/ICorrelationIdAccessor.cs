namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface ICorrelationIdAccessor
{
    string CorrelationId { get; }
}