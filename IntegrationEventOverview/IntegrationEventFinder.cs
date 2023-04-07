using MediatR;
using Microsoft.CodeAnalysis;

namespace IntegrationEventOverview;

public class IntegrationEventFinder : IIntegrationEventFinder
{
    public async Task<IEnumerable<IntegrationEventClassInformation>> FindIntegrationEvents(IEnumerable<Project> projects)
    {
        var integrationEvents = new List<IntegrationEventClassInformation>(); 
        foreach (var project in projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation == null)
            {
                throw new Exception("Compilation not found");
            }

            var classVisitor = new ClassVirtualizationVisitor(compilation, namedTypeSymbol =>
            {
                var implementedInterfaces = namedTypeSymbol.AllInterfaces;
                return implementedInterfaces.Any(symbol => symbol.Name == nameof(IIntegrationEvent) && symbol.AllInterfaces.Any(s => s.Name == nameof(INotification)));
            });

            integrationEvents.AddRange((await classVisitor.LocateInterfaces()).Select(ev =>
            {
                var ns = SyntaxHelper.GetNamespaceFrom(ev);
                return new IntegrationEventClassInformation(ev.Identifier.Text, new Namespace(ns));
            }));
        }

        return integrationEvents;
    }
}