using InfraMonitor.Domain.Enums;

namespace InfraMonitor.Domain.Entities;

public partial class Server
{
    public int ServerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Ipaddress { get; set; }

    public ServerStatus Status { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual ICollection<Disk> Disks { get; set; } = new List<Disk>();

    public virtual ICollection<Metric> Metrics { get; set; } = new List<Metric>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
