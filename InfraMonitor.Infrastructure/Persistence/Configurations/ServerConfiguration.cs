using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> entity)
    {
        entity.HasKey(e => e.ServerId).HasName("PK__Servers__C56AC8E68A010455");

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        entity.Property(e => e.Description).HasMaxLength(250);
        entity.Property(e => e.Ipaddress)
            .HasMaxLength(50)
            .HasColumnName("IPAddress");
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Status)
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasDefaultValue(InfraMonitor.Domain.Enums.ServerStatus.Up);
    }
}
