using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.EfConfigurations;

public sealed class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Token)
            .HasColumnName("token")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.EmployeeNo)
            .HasColumnName("employee_no")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ExpiresAtUtc)
            .HasColumnName("expires_at_utc");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.RevokedAtUtc)
            .HasColumnName("revoked_at_utc");

        // Hızlı lookup
        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.EmployeeNo);

        // Computed property'leri ignore et
        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsRevoked);
        builder.Ignore(x => x.IsActive);
    }
}