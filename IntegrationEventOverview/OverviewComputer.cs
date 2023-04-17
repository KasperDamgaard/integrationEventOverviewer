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

    public OverviewComputer(ILogger<OverviewComputer> logger, IVisualizer visualizer, IEventFinder eventFinder, IEventMapper eventMapper, IOverviewOutputter outputter)
    {
        _logger = logger;
        _visualizer = visualizer;
        _eventFinder = eventFinder;
        _eventMapper = eventMapper;
        _outputter = outputter;
    }

    public async Task<VisualizationOutput> ComputeOverview(CliOptions cliOptions)
    {
        var projects = (await SyntaxHelper.GetProjects(cliOptions.SolutionPath!)).ToList();
        _logger.LogInformation("Creating Event mapping for integration events");
        var integrationEventToHandlers = await CreateEventMapping(projects.ToList(), false);
        _logger.LogInformation("Creating Event mapping for domain events");
        var domainEventToHandlers = await CreateEventMapping(projects.ToList(), true);
        _logger.LogInformation("Completed Event mapping step successfully");
        var content = _visualizer.Visualize(integrationEventToHandlers, domainEventToHandlers);
        _logger.LogInformation("Completed Visualization step successfully");

        await _outputter.Output(content);
        return content;
    }

    public async Task<Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>>> CreateEventMapping(IList<Project> projects, bool findDomainEvents)
    {
        var eventText = findDomainEvents ? "Domain" : "Integration";
        _logger.LogInformation("Find all {EventText} Events", eventText);
        var events = (await _eventFinder.FindEvents(projects, findDomainEvents)).ToList();
        _logger.LogInformation("Found {Count} {EventText} Events. They are: {Events}", events.Count, eventText, events);
        var eventInfoToHandlers = new Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>>();
        foreach (var @event in events)
        {
            _logger.LogInformation("Find all {EventText} Event handler for the {EventText} Event {IntegrationEvent}", eventText, eventText, @event);
            var eventHandlers = (await _eventMapper.MapEventToHandlers(@event, projects, findDomainEvents)).ToList();
            _logger.LogInformation("Found {Count} {EventText} Event Handlers for the {EventText} Event {IntegrationEvent}", eventHandlers.Count, eventText, eventText, @event);
            eventInfoToHandlers.Add(@event, eventHandlers);
        }

        return eventInfoToHandlers;
    }
}