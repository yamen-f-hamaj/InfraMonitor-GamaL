using System;
using System.Collections.Generic;
using InfraMonitor.Domain.Enums;

namespace InfraMonitor.Domain.Entities;

public partial class Report
{
    public int ReportId { get; set; }

    public int ServerId { get; set; }
    
    public string ReportName { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ReportStatus Status { get; set; }

    public string? FilePath { get; set; }
    
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Server Server { get; set; } = null!;
}
