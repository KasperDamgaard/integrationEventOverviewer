using System.Runtime.CompilerServices;
using IntegrationEventOverviewer;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace OverviewerTests;

public abstract class TestBase
{
    protected IEnumerable<Project> Projects { get; private set; } = new List<Project>();
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
        return directory + Path.DirectorySeparatorChar + "IntegrationEventOverviewer.sln";
    }

    private static Task<IEnumerable<Project>> GetProjects()
    {
        return SyntaxHelper.GetProjects(GetThisSolutionFilePath());
    }
}