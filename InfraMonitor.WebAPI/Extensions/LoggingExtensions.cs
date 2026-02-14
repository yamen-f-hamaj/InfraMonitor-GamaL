using Serilog;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;

namespace InfraMonitor.WebAPI.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var logConnectionString = builder.Configuration.GetConnectionString("LogDatabase")!;
        var columnOptions = new Dictionary<string, ColumnWriterBase>
        {
            { "timestamp", new TimestampColumnWriter() },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "message", new RenderedMessageColumnWriter() },
            { "exception", new ExceptionColumnWriter() },
            { "request_body", new SinglePropertyColumnWriter("RequestBody", PropertyWriteMethod.ToString, NpgsqlDbType.Text) },
            { "request_headers", new SinglePropertyColumnWriter("RequestHeaders", PropertyWriteMethod.ToString, NpgsqlDbType.Text) },
            { "response_body", new SinglePropertyColumnWriter("ResponseBody", PropertyWriteMethod.ToString, NpgsqlDbType.Text) },
            { "request_time", new SinglePropertyColumnWriter("RequestTime", PropertyWriteMethod.Raw, NpgsqlDbType.TimestampTz) },
            { "response_time", new SinglePropertyColumnWriter("ResponseTime", PropertyWriteMethod.Raw, NpgsqlDbType.TimestampTz) },
            { "client_ip", new SinglePropertyColumnWriter("ClientIp", PropertyWriteMethod.ToString, NpgsqlDbType.Varchar) },
            { "http_status_code", new SinglePropertyColumnWriter("HttpStatusCode", PropertyWriteMethod.Raw, NpgsqlDbType.Integer) },
            { "duration_ms", new SinglePropertyColumnWriter("DurationMs", PropertyWriteMethod.Raw, NpgsqlDbType.Double) }
        };

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console()
            // 1. Audit Logs (Manual Audits + HTTP Request/Response)
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt =>
                    evt.Properties.TryGetValue("LogType", out var logType) &&
                    logType.ToString().Trim('"') == "AUDIT")
                .WriteTo.PostgreSQL(logConnectionString, "AuditLogs", columnOptions, needAutoCreateTable: true))
            // 2. Error/Exception Logs Only
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt =>
                    evt.Level >= Serilog.Events.LogEventLevel.Error ||
                    evt.Exception != null)
                .WriteTo.PostgreSQL(logConnectionString, "ErrorLogs", columnOptions, needAutoCreateTable: true))
            .CreateLogger();

        Log.Information("Infrastructure Monitor API is starting up...");

        Serilog.Debugging.SelfLog.Enable(Console.Error);
        builder.Host.UseSerilog();

        return builder;
    }
}
