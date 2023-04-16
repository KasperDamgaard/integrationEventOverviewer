using Microsoft.CodeAnalysis;

namespace IntegrationEventOverview;

public record EventClassInformation(string Name, Namespace Namespace)
{
    public string FullName => $"{Namespace}.{Name}";
}

public interface IEventFinder
{
    public Task<IEnumerable<EventClassInformation>> FindEvents(IEnumerable<Project> projects, bool findDomainEvents);
}