using System.Net;
using System.Text.Json;
using InfraMonitor.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InfraMonitor.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var title = "An error occurred while processing your request.";

        if (exception is NotFoundException)
        {
            code = HttpStatusCode.NotFound;
            title = "The specified resource was not found.";
        }
        else if (exception is ValidationException validationException)
        {
            code = HttpStatusCode.BadRequest;
            title = "Validation failed";
            // Return early for validation errors to include error details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            
            var validationProblemDetails = new ValidationProblemDetails(validationException.Errors)
            {
                Status = (int)code,
                Title = title,
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path
            };
            
            return context.Response.WriteAsync(JsonSerializer.Serialize(validationProblemDetails));
        }
        else if (exception is UnauthorizedException)
        {
            code = HttpStatusCode.Unauthorized;
            title = "Unauthorized";
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var problemDetails = new ProblemDetails
        {
            Status = (int)code,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        var result = JsonSerializer.Serialize(problemDetails);

        return context.Response.WriteAsync(result);
    }
}
