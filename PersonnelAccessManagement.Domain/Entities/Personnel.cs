using PersonnelAccessManagement.Domain.Common;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Domain.Entities;

public sealed class Personnel : AuditableEntity<Guid>
{
    public decimal EmployeeNo { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public Campus Campus { get; private set; }
    public Title Title { get; private set; }

    
    public ICollection<Role> Roles { get; private set; } = new List<Role>();
    
    private Personnel() { }
    
    public Personnel(decimal employeeNo, string fullName, Campus campus, Title title)
    {
        Id = Guid.NewGuid();
        EmployeeNo = employeeNo;
        FullName = fullName.Trim();
        Campus = campus;
        Title = title;
    }
    
    public void SetRoles(IEnumerable<Role> roles)
    {
        Roles.Clear();
        foreach (var role in roles)
            Roles.Add(role);
    }

    public void AddRole(Role role)
    {
        if (Roles.Any(r => r.Id == role.Id)) return;
        Roles.Add(role);
    }

    public void RemoveRole(decimal roleId)
    {
        var existing = Roles.FirstOrDefault(r => r.Id == roleId);
        if (existing is not null)
            Roles.Remove(existing);
    }
}