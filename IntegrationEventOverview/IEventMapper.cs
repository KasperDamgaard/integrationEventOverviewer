using Microsoft.CodeAnalysis;

namespace IntegrationEventOverview;

public record HandlerClassInformation(string Name, Namespace Namespace);

public interface IEventMapper
{
    public Task<IEnumerable<HandlerClassInformation>> MapEventToHandlers(EventClassInformation @event, IEnumerable<Project> projects, bool mapDomainEvents);
}