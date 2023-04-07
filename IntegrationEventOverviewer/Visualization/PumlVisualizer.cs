using System.Text;

namespace IntegrationEventOverviewer.Visualization;

public class PumlVisualizer : IVisualizer
{
    public VisualizationOutput Visualize(Dictionary<IntegrationEventClassInformation, IEnumerable<HandlerClassInformation>> integrationEventToHandlers)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@startuml");
        sb.AppendLine("!theme spacelab");
        sb.AppendLine("skin rose");
        sb.AppendLine("title Integration Event Overview");
        OrganizeByNamespace(integrationEventToHandlers).ToList().ForEach(x =>
        {
            sb.AppendLine("namespace " + x.Key.Name + "{");
            x.Value.ToList().ForEach(y =>
            {
                sb.AppendLine("class " + y.Name + "{}");
            });
            foreach (var (intEvents, handlers) in integrationEventToHandlers)
            {
                foreach (var handler in handlers)
                {
                    if (handler.Namespace.Name != x.Key.Name)
                    {
                        continue;
                    }
                    sb.AppendLine(intEvents.Name + " <-- "+handler.Name + " : Handles");
                }
            }
            sb.AppendLine("}");
        });

        sb.AppendLine("@enduml");
        return new VisualizationOutput(sb.ToString());
    }

    private static Dictionary<Namespace, IList<ClassInformation>> OrganizeByNamespace(Dictionary<IntegrationEventClassInformation, IEnumerable<HandlerClassInformation>> integrationEventToHandlers)
    {
        var namespaceToClass = new Dictionary<Namespace, IList<ClassInformation>>();
        foreach (var (intEv, handlers) in integrationEventToHandlers)
        {
            if (namespaceToClass.TryGetValue(intEv.Namespace, out var value))
            {
                value.Add(new ClassInformation(intEv.Name));
            }
            else
            {
                namespaceToClass.Add(intEv.Namespace, new List<ClassInformation> {new(intEv.Name)});
            }

            foreach (var handler in handlers)
            {
                if (namespaceToClass.TryGetValue(handler.Namespace, out value))
                {
                    value.Add(new ClassInformation(handler.Name));
                }
                else
                {
                    namespaceToClass.Add(handler.Namespace, new List<ClassInformation> {new(handler.Name)});
                }
            }
        }

        return namespaceToClass;
    }
}

internal record ClassInformation(string Name);