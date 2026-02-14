using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using Moq;

namespace InfraMonitor.Tests.Unit.Services;

/// <summary>
/// Example unit test for testing services in isolation
/// </summary>
public class ExampleServiceTests
{
    [Fact]
    public void Example_UnitTest_Pattern()
    {
        // Arrange
        var mockContext = new Mock<IApplicationDbContext>();
        // Setup your mocks here

        // Act
        var result = true; // Your service method call

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(-1, 1, 0)]
    public void Example_TheoryTest_WithMultipleInputs(int a, int b, int expected)
    {
        // Arrange & Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }
}
