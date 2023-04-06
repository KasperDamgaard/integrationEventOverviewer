using Microsoft.CodeAnalysis;

namespace IntegrationEventOverviewer;

public interface IIntegrationEventMapper
{
    public Task<IEnumerable<HandlerClassInformation>> MapIntegrationEventToHandlers(IntegrationEventClassInformation integrationEvent, IEnumerable<Project> projects);
}