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
        var sut = new EventFinder(Options.Create(new SolutionOptions()));

        // Act
        var result = await sut.FindEvents(Projects, false);
        
        // Assert
        result.ShouldContain(e => e.Name == nameof(IntegrationEventTestImplementor));
    }

    [Test]
    public async Task TestOverviewComputer()
    {
        // Arrange
        var options = Options.Create(new SolutionOptions(GetThisSolutionFilePath()));
        var visualizer = new PumlVisualizer(options);
        var sut = new OverviewComputer(LoggerFactory.CreateLogger<OverviewComputer>(), visualizer, new EventFinder(options), new EventMapper(options), new MockOutputter());
        
        // Act
        var overview = await sut.CreateEventMapping(Projects, false);
        var visualization = visualizer.Visualize(overview, new Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>>());
        
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
class IntegrationEventTestImplementor2{}
class IntegrationEventListener{}
OverviewerTests.WrongIntegrationEventTestImplementor <-- WrongIntegrationEventHandler : Handles
OverviewerTests.IntegrationEventTestImplementor <-- IntegrationEventHandler : Handles
OverviewerTests.IntegrationEventTestImplementor2 <-- IntegrationEventListener : Handles
}
@enduml

""", StringCompareShould.IgnoreLineEndings);
    }
}