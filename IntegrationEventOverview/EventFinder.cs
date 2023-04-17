using BuildingBlocks;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverview;

public class EventFinder : IEventFinder
{
    private string IntegrationEventInterfaceName { get; }
    private string DomainEventInterfaceName { get; }

    public EventFinder(IOptions<SolutionOptions> solutionOptions)
    {
        IntegrationEventInterfaceName = solutionOptions.Value.IntegrationEventInterfaceName ?? nameof(IIntegrationEvent);
        DomainEventInterfaceName = solutionOptions.Value.DomainEventInterfaceName ?? nameof(IDomainEvent);
    }

    public async Task<IEnumerable<EventClassInformation>> FindEvents(IEnumerable<Project> projects, bool findDomainEvents)
    {
        var integrationEvents = new List<EventClassInformation>(); 
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
                if (findDomainEvents)
                {
                    return implementedInterfaces.Any(symbol => symbol.Name == DomainEventInterfaceName && symbol.AllInterfaces.Any(s => s.Name != IntegrationEventInterfaceName) && symbol.AllInterfaces.Any(s => s.Name == nameof(INotification)));
                }
                return implementedInterfaces.Any(symbol => symbol.Name == IntegrationEventInterfaceName && symbol.AllInterfaces.Any(s => s.Name == nameof(INotification)));
            });

            integrationEvents.AddRange((await classVisitor.LocateInterfaces()).Select(ev =>
            {
                var ns = SyntaxHelper.GetNamespaceFrom(ev);
                return new EventClassInformation(ev.Identifier.Text, new Namespace(ns));
            }));
        }

        return integrationEvents;
    }
}