using IntegrationEventOverview.Visualization;

namespace IntegrationEventOverview.Output;

public interface IOverviewOutputter
{
    public Task Output(VisualizationOutput content);
}