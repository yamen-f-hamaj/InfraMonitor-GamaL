using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> entity)
    {
        entity.HasKey(e => e.AlertId).HasName("PK__Alerts__EBB16A8D1D08808D");

        entity.HasIndex(e => e.ServerId, "IX_Alerts_ServerId");

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        entity.Property(e => e.MetricType).HasMaxLength(50);
        entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
        entity.Property(e => e.Status)
            .HasMaxLength(20)
            .HasDefaultValue("Triggered");

        entity.HasOne(d => d.Server).WithMany(p => p.Alerts)
            .HasForeignKey(d => d.ServerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Alerts_Servers");
    }
}
