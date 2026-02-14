namespace InfraMonitor.WebAPI.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var logConnectionString = configuration.GetConnectionString("LogDatabase")!;

        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!, name: "SQLServer")
            .AddNpgSql(logConnectionString, name: "PostgreSQL-Logs");

        return services;
    }
}
