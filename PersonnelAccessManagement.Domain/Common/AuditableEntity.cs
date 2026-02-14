namespace PersonnelAccessManagement.Domain.Common;

public class AuditableEntity<TKey> : BaseEntity<TKey>
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; private set; }

    protected AuditableEntity() { }
    
    public void SoftDelete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
    }
}