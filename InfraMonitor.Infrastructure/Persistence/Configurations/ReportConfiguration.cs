using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> entity)
    {
        entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD480545356603");

        entity.Property(e => e.CompletedAt).HasColumnType("datetime");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnType("datetime");
        entity.Property(e => e.EndTime).HasColumnType("datetime");
        entity.Property(e => e.FilePath).HasMaxLength(500);
        entity.Property(e => e.StartTime).HasColumnType("datetime");
        entity.Property(e => e.Status)
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasDefaultValue(InfraMonitor.Domain.Enums.ReportStatus.Pending);

        entity.HasOne(d => d.Server).WithMany(p => p.Reports)
            .HasForeignKey(d => d.ServerId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Reports_Servers");
    }
}
