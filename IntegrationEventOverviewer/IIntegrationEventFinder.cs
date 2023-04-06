using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IntegrationEventOverviewer;

public interface IIntegrationEventFinder
{
    public Task<IEnumerable<IntegrationEventClassInformation>> FindIntegrationEvents(IEnumerable<Project> pathToSolution);
}