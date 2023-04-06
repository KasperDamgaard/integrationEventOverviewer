using IntegrationEventOverviewer;
using Shouldly;

namespace OverviewerTests;

public class IntegrationEventFinderTests : TestBase
{

    [Test]
    public async Task TestIntegrationEventFinder()
    {
        // Arrange
        var sut = new IntegrationEventFinder();

        // Act
        var result = await sut.FindIntegrationEvents(Projects);
        
        // Assert
        result.ShouldContain(e => e.Name == nameof(IntegrationEventTestImplementor));
    }

    [Test]
    public async Task TestProgram()
    {
        // Write a test for Program using arrange, act, assert
        // Arrange
        var args = new [] {"-s", GetThisSolutionFilePath()};
        
        // Act
        await Program.Main(args);
        
        // Assert
        // Assert.Pass();
    }
}