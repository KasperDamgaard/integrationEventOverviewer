using IntegrationEventOverview.Visualization;
using Microsoft.Extensions.Logging;

namespace IntegrationEventOverview.Output;

public class ConsoleOutputter : IOverviewOutputter
{
    private readonly ILogger<ConsoleOutputter> _logger;

    public ConsoleOutputter(ILogger<ConsoleOutputter> logger)
    {
        _logger = logger;
    }

    public Task Output(VisualizationOutput content)
    {
        _logger.LogInformation("Visualization output:\n{VisualizationOutput}", content.Output);
        return Task.CompletedTask;
    }
}