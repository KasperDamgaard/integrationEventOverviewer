using IntegrationEventOverview;
using IntegrationEventOverview.Visualization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        var options = new SolutionOptions(GetThisSolutionFilePath());
        var visualizer = new PumlVisualizer(Options.Create(options));
        var sut = new OverviewComputer(LoggerFactory.CreateLogger<OverviewComputer>(), visualizer, new IntegrationEventFinder(), new IntegrationEventMapper(), new MockOutputter());
        
        // Act
        var overview = await sut.CreateIntegrationEventMapping(Projects);
        var visualization = visualizer.Visualize(overview);
        
        // Assert
        visualization.Output.ShouldBe("""
@startuml
!theme spacelab
skin rose
title IntegrationEventOverview Integration Event Overview
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