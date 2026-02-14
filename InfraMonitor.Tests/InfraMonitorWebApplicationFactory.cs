using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace InfraMonitor.Tests;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// This allows you to customize the test environment.
/// </summary>
public class InfraMonitorWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // You can override services here for testing
            // For example, replace the real database with an in-memory one
            
            // Example: Remove the real DbContext and add in-memory database
            // var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            // if (descriptor != null)
            // {
            //     services.Remove(descriptor);
            // }
            // services.AddDbContext<ApplicationDbContext>(options =>
            // {
            //     options.UseInMemoryDatabase("InMemoryDbForTesting");
            // });
        });

        builder.UseEnvironment("Testing");
    }
}
