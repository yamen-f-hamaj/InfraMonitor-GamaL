using InfraMonitor.Application.Common.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace InfraMonitor.WebAPI.Services;

public class CacheInvalidator : ICacheInvalidator
{
    private readonly IOutputCacheStore _cacheStore;

    public CacheInvalidator(IOutputCacheStore cacheStore)
    {
        _cacheStore = cacheStore;
    }

    public async Task EvictByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await _cacheStore.EvictByTagAsync(tag, cancellationToken);
    }
}
