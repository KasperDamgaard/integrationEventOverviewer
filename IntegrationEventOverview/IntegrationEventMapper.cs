using MediatR;
using Microsoft.CodeAnalysis;

namespace IntegrationEventOverview;

public class IntegrationEventMapper : IIntegrationEventMapper 
{

    public async Task<IEnumerable<HandlerClassInformation>> MapIntegrationEventToHandlers(IntegrationEventClassInformation integrationEvent, IEnumerable<Project> projects)
    {
        var integrationEvents = new List<HandlerClassInformation>(); 
        foreach (var project in projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation == null)
            {
                throw new Exception("Compilation not found");
            }

            var classVisitor = new ClassVirtualizationVisitor(compilation, symbol =>
            {
                var implementedInterfaces = symbol.AllInterfaces;
                if (implementedInterfaces.Any(s => s.Name == nameof(INotificationHandler<IIntegrationEvent>) && s.TypeArguments[0].Name == integrationEvent.Name))
                {
                    return true;
                }
                return symbol.BaseType?.Name == nameof(INotificationHandler<IIntegrationEvent>) && symbol.BaseType?.TypeArguments[0].Name == integrationEvent.Name;
            });

            var handlers = (await classVisitor.LocateInterfaces()).Select(ev =>
            {
                var ns = SyntaxHelper.GetNamespaceFrom(ev);
                return new HandlerClassInformation(ev.Identifier.Text, new Namespace(ns));
            });
            integrationEvents.AddRange(handlers);
        }

        return integrationEvents;
    }
}