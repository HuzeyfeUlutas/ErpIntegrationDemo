using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class EventConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> b)
    {
        b.ToTable("Events");

        b.HasKey(x => x.Id);

        b.Property(x => x.EventType)
            .HasConversion<string>()
            .HasMaxLength(80)
            .IsRequired();

        b.Property(x => x.EmployeeNo)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.CorrelationId)
            .HasMaxLength(100)
            .IsRequired();
    }
}