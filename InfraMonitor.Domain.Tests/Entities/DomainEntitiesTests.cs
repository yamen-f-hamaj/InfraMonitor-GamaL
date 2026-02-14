using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Xunit;

namespace InfraMonitor.Domain.Tests.Entities;

public class DomainEntitiesTests
{
    [Fact]
    public void Alert_Properties_Work()
    {
        var now = DateTime.UtcNow;
        var alert = new Alert
        {
            AlertId = 1,
            ServerId = 2,
            MetricType = "Cpu",
            MetricValue = 80.5,
            Threshold = 80.0,
            Status = "Active",
            CreatedAt = now,
            ResolvedAt = now.AddMinutes(10),
            Server = new Server { ServerId = 2 }
        };

        Assert.Equal(1, alert.AlertId);
        Assert.Equal(2, alert.ServerId);
        Assert.Equal("Cpu", alert.MetricType);
        Assert.Equal(80.5, alert.MetricValue);
        Assert.Equal(80.0, alert.Threshold);
        Assert.Equal("Active", alert.Status);
        Assert.Equal(now, alert.CreatedAt);
        Assert.Equal(now.AddMinutes(10), alert.ResolvedAt);
        Assert.NotNull(alert.Server);
    }

    [Fact]
    public void Disk_Properties_Work()
    {
        var now = DateTime.UtcNow;
        var disk = new Disk
        {
            DiskId = 1,
            ServerId = 2,
            DriveLetter = "C",
            FreeSpaceMb = 1024,
            TotalSpaceMb = 2048,
            UsedPercentage = 50.0,
            Timestamp = now,
            Server = new Server { ServerId = 2 }
        };

        Assert.Equal(1, disk.DiskId);
        Assert.Equal(2, disk.ServerId);
        Assert.Equal("C", disk.DriveLetter);
        Assert.Equal(1024, disk.FreeSpaceMb);
        Assert.Equal(2048, disk.TotalSpaceMb);
        Assert.Equal(50.0, disk.UsedPercentage);
        Assert.Equal(now, disk.Timestamp);
        Assert.NotNull(disk.Server);
    }

    [Fact]
    public void Metric_Properties_Work()
    {
        var now = DateTime.UtcNow;
        var metric = new Metric
        {
            MetricId = 1,
            ServerId = 2,
            CpuUsage = 50.0,
            MemoryUsage = 40.0,
            DiskUsage = 30.0,
            ResponseTime = 10.0,
            Status = ServerStatus.Up,
            Timestamp = now,
            Server = new Server { ServerId = 2 }
        };

        Assert.Equal(1, metric.MetricId);
        Assert.Equal(2, metric.ServerId);
        Assert.Equal(50.0, metric.CpuUsage);
        Assert.Equal(40.0, metric.MemoryUsage);
        Assert.Equal(30.0, metric.DiskUsage);
        Assert.Equal(10.0, metric.ResponseTime);
        Assert.Equal(ServerStatus.Up, metric.Status);
        Assert.Equal(now, metric.Timestamp);
        Assert.NotNull(metric.Server);
    }

    [Fact]
    public void Report_Properties_Work()
    {
        var now = DateTime.UtcNow;
        var report = new Report
        {
            ReportId = 1,
            ServerId = 2,
            ReportName = "Monthly Report",
            StartTime = now.AddDays(-30),
            EndTime = now,
            Status = ReportStatus.Completed,
            FilePath = "/reports/1.pdf",
            ErrorMessage = null,
            CreatedAt = now,
            CompletedAt = now.AddSeconds(5),
            Server = new Server { ServerId = 2 }
        };

        Assert.Equal(1, report.ReportId);
        Assert.Equal(2, report.ServerId);
        Assert.Equal("Monthly Report", report.ReportName);
        Assert.Equal(now.AddDays(-30), report.StartTime);
        Assert.Equal(now, report.EndTime);
        Assert.Equal(ReportStatus.Completed, report.Status);
        Assert.Equal("/reports/1.pdf", report.FilePath);
        Assert.Null(report.ErrorMessage);
        Assert.Equal(now, report.CreatedAt);
        Assert.Equal(now.AddSeconds(5), report.CompletedAt);
        Assert.NotNull(report.Server);
    }

    [Fact]
    public void Role_Properties_Work()
    {
        var role = new Role
        {
            RoleId = 1,
            Name = "Admin",
            Description = "Administrator",
            Users = new List<User> { new User() }
        };

        Assert.Equal(1, role.RoleId);
        Assert.Equal("Admin", role.Name);
        Assert.Equal("Administrator", role.Description);
        Assert.NotEmpty(role.Users);
    }

    [Fact]
    public void Server_Properties_Work()
    {
        var now = DateTime.UtcNow;
        var server = new Server
        {
            ServerId = 1,
            Name = "Main Server",
            Ipaddress = "127.0.0.1",
            Status = ServerStatus.Up,
            Description = "Main DB",
            CreatedAt = now,
            Alerts = new List<Alert>(),
            Disks = new List<Disk>(),
            Metrics = new List<Metric>(),
            Reports = new List<Report>()
        };

        Assert.Equal(1, server.ServerId);
        Assert.Equal("Main Server", server.Name);
        Assert.Equal("127.0.0.1", server.Ipaddress);
        Assert.Equal(ServerStatus.Up, server.Status);
        Assert.Equal("Main DB", server.Description);
        Assert.Equal(now, server.CreatedAt);
        Assert.Empty(server.Alerts);
        Assert.Empty(server.Disks);
        Assert.Empty(server.Metrics);
        Assert.Empty(server.Reports);
    }

    [Fact]
    public void User_Properties_Work()
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            UserId = 1,
            UserName = "admin",
            Email = "admin@test.com",
            PasswordHash = "hash",
            RoleId = 1,
            RefreshToken = "token",
            RefreshTokenExpiry = now.AddDays(7),
            CreatedAt = now,
            Role = new Role { RoleId = 1 }
        };

        Assert.Equal(1, user.UserId);
        Assert.Equal("admin", user.UserName);
        Assert.Equal("admin@test.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal(1, user.RoleId);
        Assert.Equal("token", user.RefreshToken);
        Assert.Equal(now.AddDays(7), user.RefreshTokenExpiry);
        Assert.Equal(now, user.CreatedAt);
        Assert.NotNull(user.Role);
    }
}
