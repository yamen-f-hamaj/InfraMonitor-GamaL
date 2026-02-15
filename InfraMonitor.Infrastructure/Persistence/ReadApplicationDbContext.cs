using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Infrastructure.Persistence;

public class ReadApplicationDbContext : ApplicationDbContext, IReadApplicationDbContext
{
    // Best Practice: Disable tracking for all queries by default for performance
    public ReadApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    => ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    // Implement interface to return IQueryable (read-only)
    IQueryable<Alert> IReadApplicationDbContext.Alerts => Alerts.AsNoTracking();
    IQueryable<Disk> IReadApplicationDbContext.Disks => Disks.AsNoTracking();
    IQueryable<Metric> IReadApplicationDbContext.Metrics => Metrics.AsNoTracking();
    IQueryable<Report> IReadApplicationDbContext.Reports => Reports.AsNoTracking();
    IQueryable<Role> IReadApplicationDbContext.Roles => Roles.AsNoTracking();
    IQueryable<Server> IReadApplicationDbContext.Servers => Servers.AsNoTracking();
    IQueryable<User> IReadApplicationDbContext.Users => Users.AsNoTracking();
}
