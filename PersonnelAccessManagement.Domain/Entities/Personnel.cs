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
}