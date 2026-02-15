using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IReadApplicationDbContext
{
    IQueryable<Alert> Alerts { get; }
    IQueryable<Disk> Disks { get; }
    IQueryable<Metric> Metrics { get; }
    IQueryable<Report> Reports { get; }
    IQueryable<Role> Roles { get; }
    IQueryable<Server> Servers { get; }
    IQueryable<User> Users { get; }
}
