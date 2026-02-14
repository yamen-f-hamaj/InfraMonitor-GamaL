using System.Text;

namespace InfraMonitor.WebAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var clientIp = context.Connection.RemoteIpAddress?.ToString();

        // Capture Headers (Excluding Authorization)
        var headers = context.Request.Headers
            .Where(h => !string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(h => h.Key, h => h.Value.ToString());
        var headersJson = System.Text.Json.JsonSerializer.Serialize(headers);

        // Capture Request Body
        context.Request.EnableBuffering();
        var requestBody = await ReadStream(context.Request.Body);
        context.Request.Body.Position = 0;

        // Proxy Response Stream
        var originalBodyStream = context.Response.Body;
        using var responseBodyProxy = new MemoryStream();
        context.Response.Body = responseBodyProxy;

        await _next(context);

        // Capture Response Body
        var responseBodyText = await ReadStream(context.Response.Body);
        var endTime = DateTime.UtcNow;
        var durationMs = (endTime - startTime).TotalMilliseconds;

        _logger.LogInformation("HTTP Audit: {LogType} {RequestBody} {RequestHeaders} {ResponseBody} {RequestTime} {ResponseTime} {ClientIp} {HttpStatusCode} {DurationMs}",
            "AUDIT",
            requestBody,
            headersJson,
            responseBodyText,
            startTime,
            endTime,
            clientIp,
            context.Response.StatusCode,
            durationMs);

        await responseBodyProxy.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> ReadStream(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
        var text = await reader.ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);
        return text;
    }
}
