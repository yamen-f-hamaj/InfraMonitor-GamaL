using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace InfraMonitor.Tests.Integration.Controllers;

public class ServersControllerTests : IClassFixture<InfraMonitorWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly InfraMonitorWebApplicationFactory _factory;

    public ServersControllerTests(InfraMonitorWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetServers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - no auth token

        // Act
        var response = await _client.GetAsync("/api/v1/servers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Swagger_IsAccessibleInDevelopment()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Example of authenticated request test
    [Fact(Skip = "Requires authentication setup")]
    public async Task GetServers_WithValidToken_ReturnsServersList()
    {
        // Arrange
        var token = "your-test-jwt-token"; // You would generate this in a helper
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/servers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var servers = await response.Content.ReadFromJsonAsync<List<object>>();
        servers.Should().NotBeNull();
    }

    [Fact(Skip = "Requires authentication setup")]
    public async Task CreateServer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = "your-test-jwt-token";
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var newServer = new
        {
            Name = "Test Server",
            Ipaddress = "192.168.1.100",
            Description = "Integration test server"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/servers", newServer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
