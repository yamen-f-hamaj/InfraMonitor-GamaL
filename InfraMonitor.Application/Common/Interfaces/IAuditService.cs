namespace InfraMonitor.Application.Common.Interfaces;

public interface IAuditService
{
    void LogAudit(string action, string userId, string details);
}
