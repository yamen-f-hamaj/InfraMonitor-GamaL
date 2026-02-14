using InfraMonitor.Application;
using InfraMonitor.Infrastructure;
using InfraMonitor.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.AddSerilogLogging();

// Add Services
builder.Services.AddWebApiServices();
builder.Services.AddCorsPolicy();
builder.Services.AddHealthChecksConfiguration(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCachingAndRateLimiting(builder.Configuration);

// Add Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure Middleware Pipeline
app.ConfigureMiddlewarePipeline();

// Configure Background Jobs
app.ConfigureBackgroundJobs();

app.Run();

// Make the implicit Program class public for integration testing
public partial class Program { }
