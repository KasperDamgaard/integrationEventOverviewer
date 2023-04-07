using IntegrationEventOverviewer.Visualization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntegrationEventOverviewer.Output;

public class FileOutputter : IOverviewOutputter
{
    private readonly ILogger<FileOutputter> _logger;
    private readonly string _filePath;

    public FileOutputter(ILogger<FileOutputter> logger, IOptions<Options> options)
    {
        _logger = logger;
        var solutionPath = options.Value.SolutionPath!;
        var solutionName = solutionPath[(solutionPath.LastIndexOf('/')+1)..^".sln".Length];
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), solutionName + "-integrationEventMapping.puml");
    }

    public async Task Output(VisualizationOutput content)
    {
        _logger.LogInformation("Writing the result to {Path}", _filePath);
        await File.WriteAllTextAsync(_filePath, content.Output);
    }
}