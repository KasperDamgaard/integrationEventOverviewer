using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IntegrationEventOverview;

public record IntegrationEventClassInformation(string Name, Namespace Namespace);

public interface IIntegrationEventFinder
{
    public Task<IEnumerable<IntegrationEventClassInformation>> FindIntegrationEvents(IEnumerable<Project> pathToSolution);
}