using System.Runtime.CompilerServices;
using IntegrationEventOverviewer;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;

namespace OverviewerTests;

public class TestBase
{
    protected IEnumerable<Project> Projects { get; set; } = new List<Project>();

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
        return Program.GetProjects(GetThisSolutionFilePath());
    }
}