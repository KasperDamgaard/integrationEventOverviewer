using IntegrationEventOverview.Visualization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverview.Output;

public class FileOutputter : IOverviewOutputter
{
    private readonly ILogger<FileOutputter> _logger;
    private readonly string _filePath;

    public FileOutputter(ILogger<FileOutputter> logger, IOptions<SolutionOptions> options)
    {
        _logger = logger;
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.SolutionName + "-integrationEventMapping.puml");
    }

    public async Task Output(VisualizationOutput content)
    {
        _logger.LogInformation("Writing the result to {Path}", _filePath);
        await File.WriteAllTextAsync(_filePath, content.Output);
    }
}