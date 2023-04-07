namespace IntegrationEventOverview;

public class SolutionOptions
{
    public SolutionOptions(string solutionPath)
    {
        SolutionPath = solutionPath;
    }

    public SolutionOptions()
    {
        SolutionPath = string.Empty;
    }

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
};