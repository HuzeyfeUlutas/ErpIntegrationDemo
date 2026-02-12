using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class EventLogConfig : IEntityTypeConfiguration<EventLog>
{
    public void Configure(EntityTypeBuilder<EventLog> b)
    {
        b.ToTable("EventLogs");

        b.HasKey(x => x.Id);

        b.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();

        b.HasOne(x => x.Event)
            .WithMany(e => e.Logs)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}