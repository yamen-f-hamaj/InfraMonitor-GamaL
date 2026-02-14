using System.Linq.Expressions;
using Hangfire;
using InfraMonitor.Application.Common.Interfaces;

namespace InfraMonitor.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BackgroundJobService(IBackgroundJobClient backgroundJobClient)
        =>_backgroundJobClient = backgroundJobClient;
    
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        =>_backgroundJobClient.Enqueue(methodCall);
    
    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        =>_backgroundJobClient.Schedule(methodCall, delay);
}
