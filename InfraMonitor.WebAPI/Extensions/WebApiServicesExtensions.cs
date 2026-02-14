using System.Text.Json.Serialization;
using InfraMonitor.WebAPI.Hubs;
using InfraMonitor.WebAPI.Services;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Infrastructure.Services;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InfraMonitor.WebAPI.Extensions;

public static class WebApiServicesExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        // Controllers with JSON options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // SignalR
        services.AddSignalR();

        // Application-specific services
        services.AddScoped<IMetricsNotificationService, MetricsNotificationService>();
        services.AddScoped<ICacheInvalidator, CacheInvalidator>();
        services.AddSingleton<ISystemMetricsService, SystemMetricsService>();

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName);
        });

        return services;
    }
}
