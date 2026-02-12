using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        b.HasMany(x => x.Personnels)
            .WithMany(x => x.Roles)
            .UsingEntity(j => j.ToTable("PersonnelRoles"));
        
        b.HasMany(x => x.Rules)
            .WithMany(x => x.Roles)
            .UsingEntity(j => j.ToTable("RuleRoles"));
    }
}