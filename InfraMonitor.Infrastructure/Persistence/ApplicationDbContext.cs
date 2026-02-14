using Microsoft.EntityFrameworkCore;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Application.Common.Interfaces;

namespace InfraMonitor.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    {
    }

    public virtual DbSet<Alert> Alerts { get; set; }
    public virtual DbSet<Disk> Disks { get; set; }
    public virtual DbSet<Metric> Metrics { get; set; }
    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Server> Servers { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
