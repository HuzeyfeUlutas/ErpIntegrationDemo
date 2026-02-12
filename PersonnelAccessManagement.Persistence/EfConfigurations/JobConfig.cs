using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;


public sealed class JobConfig : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> b)
    {
        b.ToTable("Jobs");

        b.HasKey(x => x.Id);

        b.Property(x => x.JobType)
            .HasConversion<string>()
            .HasMaxLength(80)
            .IsRequired();

        b.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();
    }
}