using InfraMonitor.WebAPI.Middleware;
using InfraMonitor.WebAPI.Hubs;
using InfraMonitor.WebAPI.Filters;
using Asp.Versioning.ApiExplorer;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Infrastructure.BackgroundJobs;

namespace InfraMonitor.WebAPI.Extensions;

public static class MiddlewarePipelineExtensions
{
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Swagger (Development only)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        // Custom Middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        // Standard Middleware Pipeline
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors();

        app.UseRouting();

        app.UseRateLimiter();
        app.UseOutputCache();

        app.UseAuthentication();
        app.UseAuthorization();

        // Hangfire Dashboard
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new AllowAllHangfireAuthorizationFilter() }
        });

        // Endpoints
        app.MapControllers();
        app.MapHub<MetricsHub>("/metricsHub");

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }

    public static WebApplication ConfigureBackgroundJobs(this WebApplication app)
    {
        // Schedule Recurring Metrics Collection Job (Every 2 minutes)
        using (var scope = app.Services.CreateScope())
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            recurringJobManager.AddOrUpdate<IMetricCollector>(
                "collect-server-metrics",
                collector => collector.CollectAndStoreMetricsAsync(CancellationToken.None),
                "*/2 * * * *"); // Cron expression for every 2 minutes

            recurringJobManager.AddOrUpdate<DailyReportSchedulerJob>(
                "schedule-daily-reports",
                scheduler => scheduler.ScheduleDailyReports(CancellationToken.None),
                Cron.Daily);
        }

        return app;
    }
}
