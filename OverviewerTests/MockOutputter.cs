using IntegrationEventOverviewer.Output;
using IntegrationEventOverviewer.Visualization;

namespace OverviewerTests;

public class MockOutputter : IOverviewOutputter
{
    public Task Output(VisualizationOutput content)
    {
        return Task.CompletedTask;
    }
}