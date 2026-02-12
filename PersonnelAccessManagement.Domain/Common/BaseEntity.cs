namespace PersonnelAccessManagement.Domain.Common;

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; protected set; } = default!;
    
    protected BaseEntity() { }
}