using InfraMonitor.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace InfraMonitor.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
        =>_logger = logger;
    
    // We use structured logging with a specific "LogType" property to distinguish Audit logs in the DB
    public void LogAudit(string action, string userId, string details)
        =>_logger.LogInformation("AUDIT: {LogType} {Action} by User {UserId}. Details: {Details}", "AUDIT", action, userId, details);
}
