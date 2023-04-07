using IntegrationEventOverviewer;
using IntegrationEventOverviewer.Visualization;
using Microsoft.Extensions.Logging;
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
    public async Task TestOverviewComputer()
    {
        // Arrange
        var options = new Options {SolutionPath = GetThisSolutionFilePath()};
        var sut = new OverviewComputer(LoggerFactory.CreateLogger<OverviewComputer>(), new PumlVisualizer(), new IntegrationEventFinder(), new IntegrationEventMapper(), new MockOutputter());
        
        // Act
        var overview = await sut.ComputeOverview(options);
        
        // Assert
        overview.Output.ShouldBe("""
@startuml
!theme spacelab
skin rose
title Integration Event Overview
namespace OverviewerTests{
class WrongIntegrationEventTestImplementor{}
class WrongIntegrationEventHandler{}
class IntegrationEventTestImplementor{}
class IntegrationEventHandler{}
WrongIntegrationEventTestImplementor <-- WrongIntegrationEventHandler : Handles
IntegrationEventTestImplementor <-- IntegrationEventHandler : Handles
}
@enduml

""", StringCompareShould.IgnoreLineEndings);
    }
}