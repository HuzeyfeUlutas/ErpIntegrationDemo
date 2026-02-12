using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Rule: AuditableEntity<Guid>
{
    public string Name { get; private set; } = default!;

    public Campus? Campus { get; private set; }
    public Title? Title  { get; private set; }


    public bool IsActive { get; private set; } = true;
    
    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    
    private Rule() { }
    public Rule(string name,  Campus? campus, Title? title)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        Campus = campus;
        Title  = title;
    }
}