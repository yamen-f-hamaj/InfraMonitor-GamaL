using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace InfraMonitor.WebAPI.Extensions;

public static class CachingAndRateLimitingExtensions
{
    public static IServiceCollection AddCachingAndRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        // Output Cache
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder.Cache());
            options.AddPolicy("LatestMetrics", builder =>
                builder.Expire(TimeSpan.FromMinutes(5))
                       .Tag("metrics-latest"));
            options.AddPolicy("ServersList", builder =>
                builder.Expire(TimeSpan.FromMinutes(10))
                       .Tag("servers-list"));
        })
        .AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        // Rate Limiting
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        return services;
    }
}
