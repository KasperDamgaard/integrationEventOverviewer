using IntegrationEventOverview.Output;
using IntegrationEventOverview.Visualization;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverview;

public class OverviewComputer
{
    private readonly ILogger<OverviewComputer> _logger;
    private readonly IVisualizer _visualizer;
    private readonly IEventFinder _eventFinder;
    private readonly IEventMapper _eventMapper;
    private readonly IOverviewOutputter _outputter;
    private readonly bool _computeDomainEventOverview;
    private string EventText => _computeDomainEventOverview ? "Domain" : "Integration";

    public OverviewComputer(ILogger<OverviewComputer> logger, IVisualizer visualizer, IEventFinder eventFinder, IEventMapper eventMapper, IOverviewOutputter outputter, IOptions<SolutionOptions> options)
    {
        _logger = logger;
        _visualizer = visualizer;
        _eventFinder = eventFinder;
        _eventMapper = eventMapper;
        _outputter = outputter;
        _computeDomainEventOverview = options.Value.DomainEventOverview;
    }

    public async Task<VisualizationOutput> ComputeOverview(CliOptions cliOptions)
    {
        _logger.LogInformation("Creating {EventText} Event mapping", EventText);
        var projects = await SyntaxHelper.GetProjects(cliOptions.SolutionPath!);
        var integrationEventToHandlers = await CreateIntegrationEventMapping(projects.ToList());
        _logger.LogInformation("Completed successfully - output can be found in integrationEventMapping.txt");
        var content = _visualizer.Visualize(integrationEventToHandlers, _computeDomainEventOverview);
        
        await _outputter.Output(content);
        return content;
    }

    public async Task<Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>>> CreateIntegrationEventMapping(IList<Project> projects)
    {
        _logger.LogInformation("Find all {EventText} Events", EventText);
        var integrationEvents = (await _eventFinder.FindEvents(projects, _computeDomainEventOverview)).ToList();
        _logger.LogInformation("Found {Count} {EventText} Events. They are: {Events}", integrationEvents.Count, EventText, integrationEvents);
        var integrationEventToHandlers = new Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>>();
        foreach (var integrationEvent in integrationEvents)
        {
            _logger.LogInformation("Find all {EventText} Event handler for the {EventText} Event {IntegrationEvent}", EventText, EventText, integrationEvent);
            var integrationEventHandlers = (await _eventMapper.MapEventToHandlers(integrationEvent, projects, _computeDomainEventOverview)).ToList();
            _logger.LogInformation("Found {Count} {EventText} Event Handlers for the {EventText} Event {IntegrationEvent}", integrationEventHandlers.Count, EventText, EventText, integrationEvent);
            integrationEventToHandlers.Add(integrationEvent, integrationEventHandlers);
        }

        return integrationEventToHandlers;
    }
}