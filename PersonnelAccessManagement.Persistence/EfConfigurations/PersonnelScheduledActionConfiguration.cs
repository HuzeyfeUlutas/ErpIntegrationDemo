using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;
using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class PersonnelScheduledActionConfiguration : IEntityTypeConfiguration<PersonnelScheduledAction>
{
    public void Configure(EntityTypeBuilder<PersonnelScheduledAction> builder)
    {
        builder.ToTable("PersonnelScheduledActions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(x => x.EmployeeNo)
            .HasColumnName("employee_no")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ActionType)
            .HasColumnName("action_type")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.EffectiveDate)
            .HasColumnName("effective_date")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ScheduledActionStatus.Pending);

        builder.Property(x => x.CorrelationId)
            .HasColumnName("correlation_id")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.ProcessedAtUtc)
            .HasColumnName("processed_at_utc");

        // Idempotency: aynı event iki kez yazılmasın
        builder.HasIndex(x => x.EventId)
            .IsUnique();

        // Hangfire job'u bu index ile sorgulayacak
        builder.HasIndex(x => new { x.EffectiveDate, x.Status });

        builder.HasIndex(x => x.EmployeeNo);
    }
}