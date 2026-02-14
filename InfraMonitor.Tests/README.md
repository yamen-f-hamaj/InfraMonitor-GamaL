# InfraMonitor.Tests

Integration and API tests for the InfraMonitor application.

## Test Structure

```
InfraMonitor.Tests/
├── Integration/
│   └── Controllers/
│       ├── ServersControllerTests.cs
│       └── AuthControllerTests.cs
├── Unit/
│   └── Services/
│       └── ExampleServiceTests.cs
└── InfraMonitorWebApplicationFactory.cs
```

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run specific test class
```bash
dotnet test --filter "FullyQualifiedName~ServersControllerTests"
```

### Run with coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## Test Types

### Integration Tests
- Test the full HTTP pipeline
- Use `WebApplicationFactory` to spin up the app
- Test actual API endpoints
- Located in `Integration/` folder

### Unit Tests  
- Test individual components in isolation
- Use Moq for mocking dependencies
- Located in `Unit/` folder

## Packages Used

- **xUnit** - Test framework
- **Moq** - Mocking framework
- **FluentAssertions** - Fluent assertion library
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing support

## Notes

- Some tests are marked with `[Fact(Skip = "...")]` because they require:
  - Database setup (in-memory or test database)
  - Authentication token generation
  - Additional configuration

- To enable skipped tests, you need to:
  1. Set up test database or in-memory database
  2. Create helper methods for generating test JWT tokens
  3. Configure test-specific services in `InfraMonitorWebApplicationFactory`

## Example: Enabling Database Tests

Update `InfraMonitorWebApplicationFactory.cs`:

```csharp
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
        // Remove real database
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
        if (descriptor != null)
            services.Remove(descriptor);

        // Add in-memory database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });
    });
}
```
