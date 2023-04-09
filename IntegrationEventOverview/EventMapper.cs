using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverview;

public class EventMapper : IEventMapper 
{
    private string IntegrationEventHandlerInterfaceName { get; }

    public EventMapper(IOptions<SolutionOptions> solutionOptions)
    {
        IntegrationEventHandlerInterfaceName = solutionOptions.Value.IntegrationEventHandlerInterfaceName ?? nameof(IIntegrationEventListener<IIntegrationEvent>);
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

            var filter = IntegrationVisitorFilter(@event);
            if (mapDomainEvents)
            {
                filter = DomainVisitorFilter(@event); 
            } 
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

    private Func<INamedTypeSymbol, bool> IntegrationVisitorFilter(EventClassInformation @event)
    {
        return symbol =>
        {
            var implementedInterfaces = symbol.AllInterfaces;
            if (implementedInterfaces.Any(s => s.Name == nameof(INotificationHandler<IIntegrationEvent>) && s.TypeArguments[0].Name == @event.Name))
            {
                return true;
            }
            var match =  symbol.BaseType?.Name == nameof(INotificationHandler<IIntegrationEvent>) && symbol.BaseType?.TypeArguments[0].Name == @event.Name;
            return match || symbol.BaseType?.Name == IntegrationEventHandlerInterfaceName && symbol.BaseType?.TypeArguments[0].Name == @event.Name;
        };
    }
    
    private Func<INamedTypeSymbol, bool> DomainVisitorFilter(EventClassInformation @event)
    {
        return symbol =>
        {
            var implementedInterfaces = symbol.AllInterfaces;
            if (implementedInterfaces.Any(s => s.Name == nameof(INotificationHandler<IDomainEvent>) && s.TypeArguments[0].Name == @event.Name))
            {
                return true;
            }
            return symbol.BaseType?.Name == nameof(INotificationHandler<IDomainEvent>) && symbol.BaseType?.TypeArguments[0].Name == @event.Name;
        };
    }
}

