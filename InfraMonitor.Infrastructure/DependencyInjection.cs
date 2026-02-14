using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InfraMonitor.Infrastructure.Persistence;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Infrastructure.Services;
using InfraMonitor.Application.Common.Models;
using InfraMonitor.Infrastructure.BackgroundJobs;
using Hangfire;

namespace InfraMonitor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<AlertSettings>(configuration.GetSection(AlertSettings.SectionName));

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IThresholdChecker, ThresholdChecker>();

        // Register Metric Collector Service
        services.AddScoped<IMetricCollector, MetricCollector>();

        // Hangfire Configuration
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));

        services.AddHangfireServer();

        services.AddScoped<IReportGenerator, Services.ReportGenerator>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IReportGeneratorJob, ReportGenerationJob>();
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<DailyReportSchedulerJob>();

        return services;
    }
}
