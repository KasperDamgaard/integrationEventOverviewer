using IntegrationEventOverviewer.Visualization;

namespace IntegrationEventOverviewer.Output;

public interface IOverviewOutputter
{
    public Task Output(VisualizationOutput content);
}