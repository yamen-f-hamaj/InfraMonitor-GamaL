using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5C9E587C");

        entity.HasIndex(e => e.Email, "UQ__Users__A9D1053400C7BA05").IsUnique();

        entity.HasIndex(e => e.UserName, "UQ__Users__C9F284569C71F078").IsUnique();

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        entity.Property(e => e.Email).HasMaxLength(100);
        entity.Property(e => e.PasswordHash).HasMaxLength(500);
        entity.Property(e => e.RefreshToken).HasMaxLength(500);
        entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
        entity.Property(e => e.UserName).HasMaxLength(50);

        entity.HasOne(d => d.Role).WithMany(p => p.Users)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_Roles");
    }
}
