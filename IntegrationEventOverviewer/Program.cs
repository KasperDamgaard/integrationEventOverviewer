// See https://aka.ms/new-console-template for more information

using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace IntegrationEventOverviewer;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Handle input as the path to a solution file using CommandLineParser
        var options = Parser.Default.ParseArguments<Options>(args).Value
            ?? throw new ArgumentException("No options provided");
        // TODO: KLD: Move to some base class which set's up stuff
        try
        {
            MSBuildLocator.RegisterDefaults();
        } catch (InvalidOperationException)
        {
            // Ignore
        }
        Console.WriteLine("Creating Integration Event mapping");
        var projects = await GetProjects(options.SolutionPath!);
        await CreateIntegrationEventMapping(projects.ToList());
        Console.WriteLine("Completed successfully - output can be found in integrationEventMapping.txt");
    }

    public static async Task<IEnumerable<Project>> GetProjects(string pathToSolution)
    {
        if (!File.Exists(pathToSolution) || !pathToSolution.EndsWith(".sln"))
        {
            Console.Error.WriteLine("{0} is not a valid file", pathToSolution);
            throw new ArgumentException("Illegal argument - not a valid file", nameof(pathToSolution));
        }

        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(pathToSolution);
        return solution.Projects;
    }

    public static async Task CreateIntegrationEventMapping(IList<Project> projects)
    {
        var eventFinder = new IntegrationEventFinder();
        var integrationEvents = await eventFinder.FindIntegrationEvents(projects);
        var eventMapper = new IntegrationEventMapper();
        var integrationEventToHandlers = new Dictionary<IntegrationEventClassInformation, IEnumerable<HandlerClassInformation>>();
        foreach (var integrationEvent in integrationEvents)
        {
            var integrationEventHandlers = await eventMapper.MapIntegrationEventToHandlers(integrationEvent, projects);
            integrationEventToHandlers.Add(integrationEvent, integrationEventHandlers);
        }

        var visualizer = new PumlVisualizer();
        var content = visualizer.Visualize(integrationEventToHandlers);
        await File.WriteAllTextAsync("integrationEventMapping.puml", content);
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class Options
{
    [Option('s', "solution", Required = true, HelpText = "Path to the solution file")]
    public string? SolutionPath { get; set; }
}