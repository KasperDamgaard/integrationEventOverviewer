using IntegrationEventOverview;
using Microsoft.Extensions.Options;
using Shouldly;

namespace OverviewerTests;

public class IntegrationEventMapperTest : TestBase
{
    [Test]
    public async Task ImplementorMapsToHandlerInterface()
    {
        // Arrange
        var sut = new EventFinder(EmptyOptions);
        var result = (await sut.FindEvents(Projects, false)).ToList();
        result.ShouldNotBeEmpty();
        var integrationEvent = result.First(x => x.Name == nameof(IntegrationEventTestImplementor2)); 
        var mapper = new EventMapper(EmptyOptions);
        
        // Act
        var mapping = await mapper.MapEventToHandlers(integrationEvent, Projects, false);
        
        // Assert
        mapping.ShouldContain(x => x.Name == nameof(IntegrationEventListener));
    }
    
    [Test]
    public async Task ImplementorMapsToHandler()
    {
        // Arrange
        var sut = new EventFinder(EmptyOptions);
        var result = (await sut.FindEvents(Projects, false)).ToList();
        result.ShouldNotBeEmpty();
        var integrationEvent = result.First(x => x.Name == nameof(IntegrationEventTestImplementor)); 
        var mapper = new EventMapper(EmptyOptions);
        
        // Act
        var mapping = await mapper.MapEventToHandlers(integrationEvent, Projects, false);
        
        // Assert
        mapping.ShouldContain(x => x.Name == nameof(IntegrationEventHandler));
    }
    
    [Test]
    public async Task ImplementorDoesNotMapToWrongHandler()
    {
        // Arrange
        var integrationEventFinder = new EventFinder(EmptyOptions);
        var result = (await integrationEventFinder.FindEvents(Projects, false)).ToList();
        result.ShouldNotBeEmpty();
        var integrationEvent = result.Find(x => x.Name == nameof(IntegrationEventTestImplementor)); 
        var mapper = new EventMapper(EmptyOptions);
        
        // Act
        var mapping = await mapper.MapEventToHandlers(integrationEvent!, Projects, false);
        
        // Assert
        mapping.ShouldNotContain(x => x.Name == nameof(WrongIntegrationEventHandler));
    }
}