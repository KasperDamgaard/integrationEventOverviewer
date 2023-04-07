using IntegrationEventOverview.Output;
using IntegrationEventOverview.Visualization;

namespace OverviewerTests;

public class MockOutputter : IOverviewOutputter
{
    public Task Output(VisualizationOutput content)
    {
        return Task.CompletedTask;
    }
}