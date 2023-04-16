using System.Text;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverview.Visualization;

public class PumlVisualizer : IVisualizer
{
    private readonly SolutionOptions _solutionOptions;

    public PumlVisualizer(IOptions<SolutionOptions> options)
    { 
        _solutionOptions = options.Value;
    }

    public VisualizationOutput Visualize(Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>> integrationEventToHandlers, Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>> domainEventToHandlers)
    {
        var popularDomainEvents = domainEventToHandlers.Where(x => x.Value.Count() > 2)
            .ToDictionary(x => x.Key, x => x.Value);
        var lessPopularDomainEvents = domainEventToHandlers.Where(x => x.Value.Any() && x.Value.Count() < 2)
            .ToDictionary(x => x.Key, x => x.Value);

        var lonelyDomainEvents = domainEventToHandlers.Where(x => !x.Value.Any())
            .ToDictionary( x => x.Key, x => x.Value);

        var diagrams = new List<string?>
        {
            WriteDiagram(integrationEventToHandlers, $"{_solutionOptions.SolutionName} Integration Event Overview"),
            WriteDiagram(popularDomainEvents,
                $"{_solutionOptions.SolutionName} Domain Event Overview: Highly connected (>2)"),
            WriteDiagram(lessPopularDomainEvents,
                $"{_solutionOptions.SolutionName} Domain Event Overview: Less connected (1-2)"),
            WriteDiagram(lonelyDomainEvents, $"{_solutionOptions.SolutionName} Domain Event Overview: Lonely (0)"),
        };
        diagrams.RemoveAll(string.IsNullOrEmpty);
        var sb = new StringBuilder();
        sb.AppendLine("@startuml");
        sb.AppendJoin("newpage" + Environment.NewLine, diagrams);
        sb.AppendLine("@enduml");
        return new VisualizationOutput(sb.ToString());
    }

    private static string? WriteDiagram(Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>> integrationEventToHandlers, string title)
    {
        if (integrationEventToHandlers.Count == 0)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.AppendLine("!theme spacelab");
        sb.AppendLine("skin rose");
        sb.AppendLine($"title {title}");
        OrganizeByNamespace(integrationEventToHandlers).ToList().ForEach(classesPerNs =>
        {
            sb.AppendLine("namespace " + classesPerNs.Key.Name + "{");
            classesPerNs.Value.ToList().ForEach(classInformation =>
            {
                sb.AppendLine("class " + classInformation.Name + "{}");
            });
            foreach (var (intEvents, handlers) in integrationEventToHandlers)
            {
                foreach (var handler in handlers)
                {
                    if (handler.Namespace.Name != classesPerNs.Key.Name)
                    {
                        continue;
                    }

                    sb.AppendLine(intEvents.FullName + " <-- " + handler.Name + " : Handles");
                }
            }

            sb.AppendLine("}");
        });
        return sb.ToString();
    }

    private static Dictionary<Namespace, IList<ClassInformation>> OrganizeByNamespace(Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>> integrationEventToHandlers)
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