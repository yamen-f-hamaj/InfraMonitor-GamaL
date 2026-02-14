using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> entity)
    {
        entity.HasKey(e => e.MetricId).HasName("PK__Metrics__561056A5E8CCBF80");

        entity.HasIndex(e => new { e.ServerId, e.Timestamp }, "IX_Metrics_ServerId_Timestamp");

        entity.Property(e => e.Status)
            .HasMaxLength(20)
            .HasConversion<string>();
        entity.Property(e => e.Timestamp)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");

        entity.HasOne(d => d.Server).WithMany(p => p.Metrics)
            .HasForeignKey(d => d.ServerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Metrics_Servers");
    }
}
