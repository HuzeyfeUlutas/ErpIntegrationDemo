using PersonnelAccessManagement.Domain.Common;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Role : AuditableEntity<decimal>
{
    public string Name { get; private set; } = default!;
    
    public ICollection<Personnel> Personnels { get; private set; } = new List<Personnel>();
    public ICollection<Rule> Rules { get; private set; } = new List<Rule>();
    
    private Role() { }

    public Role(decimal id, string name)
    {
        Id = id;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
}