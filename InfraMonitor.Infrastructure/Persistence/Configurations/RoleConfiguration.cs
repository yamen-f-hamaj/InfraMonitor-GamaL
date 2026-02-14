using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AAFBAB3F9");

        entity.HasIndex(e => e.Name, "UQ__Roles__737584F6779F8BB7").IsUnique();

        entity.Property(e => e.Description).HasMaxLength(250);
        entity.Property(e => e.Name).HasMaxLength(50);
    }
}
