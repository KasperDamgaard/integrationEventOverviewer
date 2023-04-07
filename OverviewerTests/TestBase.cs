using System.Runtime.CompilerServices;
using IntegrationEventOverview;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;

namespace OverviewerTests;

public abstract class TestBase
{
    protected IList<Project> Projects { get; private set; } = new List<Project>();
    protected readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(b => b.AddSimpleConsole(o => o.IncludeScopes = true));

    [SetUp]
    public async Task Setup()
    {
        try
        {
            MSBuildLocator.RegisterDefaults();
        } catch (InvalidOperationException)
        {
            // Ignore
        }
        Projects = await GetProjects();
    }
    
    protected static string GetThisSolutionFilePath([CallerFilePath] string path = null!)
    {
        var directory = Directory.GetParent(Path.GetDirectoryName(path)!)!.FullName;
        return directory + Path.DirectorySeparatorChar + "IntegrationEventOverview.sln";
    }

    private static async Task<IList<Project>> GetProjects()
    {
        var pathToSolution = GetThisSolutionFilePath();
        if (!File.Exists(pathToSolution) || !pathToSolution.EndsWith(".sln"))
        {
            throw new ArgumentException($"Illegal argument - {pathToSolution} is not a valid file", nameof(pathToSolution));
        }

        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);
        return solution.Projects.ToList();
    }
}