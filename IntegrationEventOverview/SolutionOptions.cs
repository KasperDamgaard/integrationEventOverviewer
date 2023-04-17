namespace IntegrationEventOverview;

public class SolutionOptions
{
    public string SolutionPath { get; set; }
    public string SolutionName
    {
        get
        {
            var inx = SolutionPath.LastIndexOf('/');
            if (inx == -1)
            {
                inx = SolutionPath.LastIndexOf('\\');
            }
            return SolutionPath[(inx + 1)..^".sln".Length];
        }
    }

    public string? IntegrationEventInterfaceName { get; set; }
    public string? IntegrationEventHandlerInterfaceName { get; set; }
    public string? DomainEventInterfaceName { get; set; }
    public string? DomainEventHandlerInterfaceName { get; set; }

    public SolutionOptions(string solutionPath)
    {
        SolutionPath = solutionPath;
    }

    public SolutionOptions()
    {
        SolutionPath = string.Empty;
    }
};