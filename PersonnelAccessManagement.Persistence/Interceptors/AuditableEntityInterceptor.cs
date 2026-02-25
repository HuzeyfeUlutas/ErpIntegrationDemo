using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Common;

namespace PersonnelAccessManagement.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserAccessor _currentUser;

    public AuditableEntityInterceptor(ICurrentUserAccessor currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void SetAuditFields(DbContext? context)
    {
        if (context is null) return;

        var employeeNo = _currentUser.EmployeeNo;
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var entityType = entry.Entity.GetType().BaseType;
        
            if (entityType is null || !entityType.IsGenericType || 
                entityType.GetGenericTypeDefinition() != typeof(AuditableEntity<>))
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property("CreatedAt").CurrentValue = now;
                    entry.Property("CreatedBy").CurrentValue = employeeNo ?? "System";
                    break;
                case EntityState.Modified:
                    entry.Property("UpdatedAt").CurrentValue = now;
                    entry.Property("UpdatedBy").CurrentValue = employeeNo ?? "System";
                    break;
            }
        }
    }
}