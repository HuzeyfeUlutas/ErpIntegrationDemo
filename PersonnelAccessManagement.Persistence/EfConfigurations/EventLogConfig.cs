using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
{
    public void Configure(EntityTypeBuilder<EventLog> builder)
    {
        builder.ToTable("EventLogs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PersonnelName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.RoleName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Action).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Error).HasMaxLength(2000);
        builder.HasIndex(e => e.EventId);
        builder.HasIndex(e => e.EmployeeNo);
    }
}