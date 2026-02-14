using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class DiskConfiguration : IEntityTypeConfiguration<Disk>
{
    public void Configure(EntityTypeBuilder<Disk> entity)
    {
        entity.HasKey(e => e.DiskId).HasName("PK__Disks__1AC118DDEC88105B");

        entity.HasIndex(e => new { e.ServerId, e.Timestamp }, "IX_Disks_ServerId_Timestamp");

        entity.Property(e => e.DriveLetter).HasMaxLength(5);
        entity.Property(e => e.FreeSpaceMb).HasColumnName("FreeSpaceMB");
        entity.Property(e => e.Timestamp)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        entity.Property(e => e.TotalSpaceMb).HasColumnName("TotalSpaceMB");

        entity.HasOne(d => d.Server).WithMany(p => p.Disks)
            .HasForeignKey(d => d.ServerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Disks_Servers");
    }
}
