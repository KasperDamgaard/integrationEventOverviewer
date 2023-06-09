﻿namespace IntegrationEventOverview.Visualization;

public record VisualizationOutput(string Output);

public interface IVisualizer
{
    public VisualizationOutput Visualize(Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>> integrationEventToHandlers, Dictionary<EventClassInformation, IEnumerable<HandlerClassInformation>> domainEventToHandlers);
}