using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IntegrationEventOverviewer;

public interface IIntegrationEventFinder
{
    public Task<IEnumerable<ClassDeclarationSyntax>> FindIntegrationEventImplementors(string pathToSolution);
}