using BuildingBlocks;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverview;

public class EventMapper : IEventMapper 
{
    private string IntegrationEventHandlerInterfaceName { get; }
    private string DomainEventHandlerInterfaceName { get; }

    public EventMapper(IOptions<SolutionOptions> solutionOptions)
    {
        IntegrationEventHandlerInterfaceName = solutionOptions.Value.IntegrationEventHandlerInterfaceName ?? nameof(IIntegrationEventListener<IIntegrationEvent>);
        DomainEventHandlerInterfaceName = solutionOptions.Value.DomainEventHandlerInterfaceName ?? nameof(IDomainEventListener<IDomainEvent>);
    }

    public async Task<IEnumerable<HandlerClassInformation>> MapEventToHandlers(EventClassInformation @event, IEnumerable<Project> projects, bool mapDomainEvents)
    {
        var integrationEvents = new List<HandlerClassInformation>(); 
        foreach (var project in projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation == null)
            {
                throw new Exception("Compilation not found");
            }

            var filter = VisitorFilter(@event, mapDomainEvents ? DomainEventHandlerInterfaceName : IntegrationEventHandlerInterfaceName);
            var classVisitor = new ClassVirtualizationVisitor(compilation, filter);

            var handlers = (await classVisitor.LocateInterfaces()).Select(ev =>
            {
                var ns = SyntaxHelper.GetNamespaceFrom(ev);
                return new HandlerClassInformation(ev.Identifier.Text, new Namespace(ns));
            });
            integrationEvents.AddRange(handlers);
        }

        return integrationEvents;
    }

    private Func<INamedTypeSymbol, bool> VisitorFilter(EventClassInformation @event, string interfaceName)
    {
        return symbol =>
        {
            var implementedInterfaces = symbol.AllInterfaces;
            if (implementedInterfaces.Any(s => s.Name == nameof(INotificationHandler<IIntegrationEvent>) && s.TypeArguments[0].Name == @event.Name))
            {
                return true;
            }
            var match =  symbol.BaseType?.Name == nameof(INotificationHandler<IIntegrationEvent>) && symbol.BaseType?.TypeArguments[0].Name == @event.Name;
            return match || symbol.BaseType?.Name == interfaceName && symbol.BaseType?.TypeArguments[0].Name == @event.Name;
        };
    }
}

