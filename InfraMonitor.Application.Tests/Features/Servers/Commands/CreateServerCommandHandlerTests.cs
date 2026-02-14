using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Servers.Commands;

public class CreateServerCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly CreateServerCommandHandler _handler;
    private readonly List<Server> _servers;

    public CreateServerCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _servers = new List<Server>();

        var mockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_servers);
        _mockContext.Setup(c => c.Servers).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new CreateServerCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesServerWithCorrectProperties()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.100",
            Description: "Test Description"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _servers.Should().HaveCount(1);
        var createdServer = _servers.First();
        
        createdServer.Name.Should().Be("Test Server");
        createdServer.Ipaddress.Should().Be("192.168.1.100");
        createdServer.Description.Should().Be("Test Description");
        createdServer.Status.Should().Be(ServerStatus.Up);
        createdServer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChangesAsync()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.100",
            Description: "Test Description"
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsServerId()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.100",
            Description: "Test Description"
        );

        // Mock the ServerId assignment
        _mockContext.Setup(c => c.Servers.Add(It.IsAny<Server>()))
            .Callback<Server>(s => s.ServerId = 123);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(123);
    }

    [Fact]
    public async Task Handle_CommandWithNullIpAddress_CreatesServerSuccessfully()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: null,
            Description: "Test Description"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _servers.Should().HaveCount(1);
        _servers.First().Ipaddress.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CommandWithNullDescription_CreatesServerSuccessfully()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.100",
            Description: null
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _servers.Should().HaveCount(1);
        _servers.First().Description.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DefaultStatus_IsSetToUp()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.100",
            Description: "Test"
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _servers.First().Status.Should().Be(ServerStatus.Up);
    }
}
