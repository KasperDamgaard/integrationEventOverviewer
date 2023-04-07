using Microsoft.CodeAnalysis;

namespace IntegrationEventOverview;

public record HandlerClassInformation(string Name, Namespace Namespace);

public interface IIntegrationEventMapper
{
    public Task<IEnumerable<HandlerClassInformation>> MapIntegrationEventToHandlers(IntegrationEventClassInformation integrationEvent, IEnumerable<Project> projects);
}