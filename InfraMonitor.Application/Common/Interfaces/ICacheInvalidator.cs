namespace InfraMonitor.Application.Common.Interfaces;

public interface ICacheInvalidator
{
    Task EvictByTagAsync(string tag, CancellationToken cancellationToken = default);
}
