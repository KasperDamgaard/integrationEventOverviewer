using IntegrationEventOverviewer.Output;
using IntegrationEventOverviewer.Visualization;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace IntegrationEventOverviewer;

public class OverviewComputer
{
    private readonly ILogger<OverviewComputer> _logger;
    private readonly IVisualizer _visualizer;
    private readonly IIntegrationEventFinder _eventFinder;
    private readonly IIntegrationEventMapper _eventMapper;
    private readonly IOverviewOutputter _outputter;

    public OverviewComputer(ILogger<OverviewComputer> logger, IVisualizer visualizer, IIntegrationEventFinder eventFinder, IIntegrationEventMapper eventMapper, IOverviewOutputter outputter)
    {
        _logger = logger;
        _visualizer = visualizer;
        _eventFinder = eventFinder;
        _eventMapper = eventMapper;
        _outputter = outputter;
    }

    public async Task<VisualizationOutput> ComputeOverview(Options options)
    {
        _logger.LogInformation("Creating Integration Event mapping");
        var projects = await SyntaxHelper.GetProjects(options.SolutionPath!);
        var integrationEventToHandlers = await CreateIntegrationEventMapping(projects.ToList());
        _logger.LogInformation("Completed successfully - output can be found in integrationEventMapping.txt");
        var content = _visualizer.Visualize(integrationEventToHandlers);
        
        await _outputter.Output(content);
        return content;
    }

    private async Task<Dictionary<IntegrationEventClassInformation, IEnumerable<HandlerClassInformation>>> CreateIntegrationEventMapping(IList<Project> projects)
    {
        _logger.LogInformation("Find all Integration Events");
        var integrationEvents = (await _eventFinder.FindIntegrationEvents(projects)).ToList();
        _logger.LogInformation("Found {Count} Integration Events. They are: {Events}", integrationEvents.Count, integrationEvents);
        var integrationEventToHandlers = new Dictionary<IntegrationEventClassInformation, IEnumerable<HandlerClassInformation>>();
        foreach (var integrationEvent in integrationEvents)
        {
            _logger.LogInformation("Find all Integration Event handler for the Integration Event {IntegrationEvent}", integrationEvent);
            var integrationEventHandlers = await _eventMapper.MapIntegrationEventToHandlers(integrationEvent, projects);
            _logger.LogInformation("Found {Count} Integration Event Handlers for the Integration Event {IntegrationEvent}", integrationEvents.Count, integrationEvent);
            integrationEventToHandlers.Add(integrationEvent, integrationEventHandlers);
        }

        return integrationEventToHandlers;
    }
}