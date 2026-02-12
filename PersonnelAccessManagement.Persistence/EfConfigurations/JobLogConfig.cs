using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class JobLogConfig : IEntityTypeConfiguration<JobLog>
{
    public void Configure(EntityTypeBuilder<JobLog> b)
    {
        b.ToTable("JobLogs");

        b.HasKey(x => x.Id);

        b.Property(x => x.Message)
            .HasMaxLength(1000)
            .IsRequired();

        b.HasOne(x => x.Job)
            .WithMany(j => j.Logs)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}