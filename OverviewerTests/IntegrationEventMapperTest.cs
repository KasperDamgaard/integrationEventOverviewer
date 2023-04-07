using IntegrationEventOverview;
using Shouldly;

namespace OverviewerTests;

public class IntegrationEventMapperTest : TestBase
{
    [Test]
    public async Task ImplementorMapsToHandler()
    {
        // Arrange
        var sut = new IntegrationEventFinder();
        var result = (await sut.FindIntegrationEvents(Projects)).ToList();
        result.ShouldNotBeEmpty();
        var integrationEvent = result.First(x => x.Name == nameof(IntegrationEventTestImplementor)); 
        var mapper = new IntegrationEventMapper();
        
        // Act
        var mapping = await mapper.MapIntegrationEventToHandlers(integrationEvent, Projects);
        
        // Assert
        mapping.ShouldContain(x => x.Name == nameof(IntegrationEventHandler));
    }
    
    [Test]
    public async Task ImplementorDoesNotMapToWrongHandler()
    {
        // Arrange
        var integrationEventFinder = new IntegrationEventFinder();
        var result = (await integrationEventFinder.FindIntegrationEvents(Projects)).ToList();
        result.ShouldNotBeEmpty();
        var integrationEvent = result.Find(x => x.Name == nameof(IntegrationEventTestImplementor)); 
        var mapper = new IntegrationEventMapper();
        
        // Act
        var mapping = await mapper.MapIntegrationEventToHandlers(integrationEvent!, Projects);
        
        // Assert
        mapping.ShouldNotContain(x => x.Name == nameof(WrongIntegrationEventHandler));
    }
}