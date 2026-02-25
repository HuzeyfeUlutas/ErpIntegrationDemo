using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class KafkaEventLogConfiguration : IEntityTypeConfiguration<KafkaEventLog>
{
    public void Configure(EntityTypeBuilder<KafkaEventLog> builder)
    {
        builder.ToTable("KafkaEventLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Topic)
            .HasColumnName("topic")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.PartitionNo)
            .HasColumnName("partition_no");

        builder.Property(x => x.Offset)
            .HasColumnName("offset");

        builder.Property(x => x.MessageKey)
            .HasColumnName("message_key")
            .HasMaxLength(512);

        builder.Property(x => x.MessageValue)
            .HasColumnName("message_value");

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .IsRequired();

        builder.Property(x => x.ErrorStackTrace)
            .HasColumnName("error_stack_trace");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("FAILED");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}