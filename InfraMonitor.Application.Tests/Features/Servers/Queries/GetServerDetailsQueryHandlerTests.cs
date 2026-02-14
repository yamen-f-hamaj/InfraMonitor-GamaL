using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Servers.Queries;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Servers.Queries;

public class GetServerDetailsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly GetServerDetailsQueryHandler _handler;
    private readonly List<Server> _servers;

    public GetServerDetailsQueryHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _servers = new List<Server>
        {
            new Server
            {
                ServerId = 1,
                Name = "Test Server",
                Ipaddress = "192.168.1.1",
                Description = "Test Description",
                Status = ServerStatus.Up,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Metrics = new List<Metric>
                {
                    new Metric
                    {
                        MetricId = 1,
                        CpuUsage = 50.5,
                        MemoryUsage = 60.0,
                        DiskUsage = 70.0,
                        ResponseTime = 100.0,
                        Status = ServerStatus.Up,
                        Timestamp = DateTime.UtcNow.AddMinutes(-5)
                    },
                    new Metric
                    {
                        MetricId = 2,
                        CpuUsage = 55.5,
                        MemoryUsage = 65.0,
                        DiskUsage = 75.0,
                        ResponseTime = 110.0,
                        Status = ServerStatus.Up,
                        Timestamp = DateTime.UtcNow.AddMinutes(-10)
                    }
                }
            }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_servers);
        _mockContext.Setup(c => c.Servers).Returns(mockDbSet.Object);

        _handler = new GetServerDetailsQueryHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ValidServerId_ReturnsServerDetails()
    {
        // Arrange
        var query = new GetServerDetailsQuery(ServerId: 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ServerId.Should().Be(1);
        result.Name.Should().Be("Test Server");
        result.Ipaddress.Should().Be("192.168.1.1");
        result.Description.Should().Be("Test Description");
        result.Status.Should().Be(ServerStatus.Up);
    }

    [Fact]
    public async Task Handle_ValidServerId_ReturnsRecentMetrics()
    {
        // Arrange
        var query = new GetServerDetailsQuery(ServerId: 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.RecentMetrics.Should().HaveCount(2);
        
        // Metrics should be ordered by timestamp descending
        result.RecentMetrics.First().MetricId.Should().Be(1); // Most recent
        result.RecentMetrics.Last().MetricId.Should().Be(2);
    }

    [Fact]
    public async Task Handle_NonExistentServerId_ReturnsNull()
    {
        // Arrange
        var query = new GetServerDetailsQuery(ServerId: 999);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ServerWithNoMetrics_ReturnsEmptyMetricsList()
    {
        // Arrange
        var serverWithoutMetrics = new Server
        {
            ServerId = 2,
            Name = "Server without metrics",
            Ipaddress = "192.168.1.2",
            Status = ServerStatus.Up,
            CreatedAt = DateTime.UtcNow,
            Metrics = new List<Metric>()
        };
        _servers.Add(serverWithoutMetrics);

        var query = new GetServerDetailsQuery(ServerId: 2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.RecentMetrics.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsServerDetailsDto()
    {
        // Arrange
        var query = new GetServerDetailsQuery(ServerId: 1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ServerDetailsDto>();
    }
}
