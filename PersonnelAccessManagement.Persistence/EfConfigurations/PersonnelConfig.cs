using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class PersonnelConfig : IEntityTypeConfiguration<Personnel>
{
    public void Configure(EntityTypeBuilder<Personnel> b)
    {
        b.ToTable("Personnels");

        b.HasKey(x => x.Id);

        b.Property(x => x.EmployeeNo)
            .IsRequired();

        b.HasIndex(x => x.EmployeeNo)
            .IsUnique();

        b.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();
        
        b.Property(x => x.Campus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.Title)
            .HasConversion<string>()
            .HasMaxLength(80)
            .IsRequired();
    }
}