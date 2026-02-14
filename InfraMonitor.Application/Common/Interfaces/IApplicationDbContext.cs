using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Alert> Alerts { get; set; }
    DbSet<Disk> Disks { get; set; }
    DbSet<Metric> Metrics { get; set; }
    DbSet<Report> Reports { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<Server> Servers { get; set; }
    DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
