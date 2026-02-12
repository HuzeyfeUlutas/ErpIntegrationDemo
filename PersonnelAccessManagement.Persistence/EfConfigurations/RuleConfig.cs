using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class RuleConfig : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> b)
    {
        b.ToTable("Rules");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        b.Property(x => x.Campus)
            .HasConversion<string>()
            .HasMaxLength(50);

        b.Property(x => x.Title)
            .HasConversion<string>()
            .HasMaxLength(80);

        b.Property(x => x.IsActive)
            .IsRequired();
        
        b.HasIndex(x => new { x.Campus, x.Title });
    }
}