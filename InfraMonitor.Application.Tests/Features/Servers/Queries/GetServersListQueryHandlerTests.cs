using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Servers.Queries;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Servers.Queries;

public class GetServersListQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly GetServersListQueryHandler _handler;
    private readonly List<Server> _servers;

    public GetServersListQueryHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _servers = new List<Server>
        {
            new Server
            {
                ServerId = 1,
                Name = "Server 1",
                Ipaddress = "192.168.1.1",
                Description = "First server",
                Status = ServerStatus.Up,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Server
            {
                ServerId = 2,
                Name = "Server 2",
                Ipaddress = "192.168.1.2",
                Description = "Second server",
                Status = ServerStatus.Down,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Server
            {
                ServerId = 3,
                Name = "Server 3",
                Ipaddress = null,
                Description = null,
                Status = ServerStatus.Maintenance,
                CreatedAt = DateTime.UtcNow
            }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_servers);
        _mockContext.Setup(c => c.Servers).Returns(mockDbSet.Object);

        _handler = new GetServersListQueryHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllServers()
    {
        // Arrange
        var query = new GetServersListQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_ReturnsServersWithCorrectProperties()
    {
        // Arrange
        var query = new GetServersListQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var firstServer = result.First(s => s.ServerId == 1);
        firstServer.Name.Should().Be("Server 1");
        firstServer.Ipaddress.Should().Be("192.168.1.1");
        firstServer.Description.Should().Be("First server");
        firstServer.Status.Should().Be(ServerStatus.Up);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var emptyServers = new List<Server>();
        var emptyMockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(emptyServers);
        _mockContext.Setup(c => c.Servers).Returns(emptyMockDbSet.Object);
        
        var handler = new GetServersListQueryHandler(_mockContext.Object);
        var query = new GetServersListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsServerDtoType()
    {
        // Arrange
        var query = new GetServersListQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().AllBeOfType<ServerDto>();
    }

    [Fact]
    public async Task Handle_HandlesNullValues()
    {
        // Arrange
        var query = new GetServersListQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var serverWithNulls = result.First(s => s.ServerId == 3);
        serverWithNulls.Ipaddress.Should().BeNull();
        serverWithNulls.Description.Should().BeNull();
    }
}
