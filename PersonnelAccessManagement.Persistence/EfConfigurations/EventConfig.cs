using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.SourceId).HasMaxLength(128).IsRequired();
        builder.Property(e => e.SourceDetail).HasColumnType("jsonb");
        builder.Property(e => e.CorrelationId).HasMaxLength(64).IsRequired();
        builder.HasMany(e => e.Logs).WithOne(l => l.Event).HasForeignKey(l => l.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(e => e.CorrelationId);
        builder.HasIndex(e => new { e.EventType, e.IsCompleted });
    }
}