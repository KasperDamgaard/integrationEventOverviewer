// See https://aka.ms/new-console-template for more information

using CommandLine;
using IntegrationEventOverviewer.Output;
using IntegrationEventOverviewer.Visualization;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationEventOverviewer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        // TODO: KLD: Move to some base class which set's up stuff
        try
        {
            MSBuildLocator.RegisterDefaults();
        } catch (InvalidOperationException)
        {
            // Ignore
        }
        // Handle input as the path to a solution file using CommandLineParser
        var options = Parser.Default.ParseArguments<Options>(args).Value
                      ?? throw new ArgumentException("No options provided");
        var workerInstance = host.Services.GetRequiredService<OverviewComputer>();
        await workerInstance.ComputeOverview(options);
        await host.StopAsync();
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddTransient<OverviewComputer>();
                services.AddTransient<IVisualizer, PumlVisualizer>();
                services.AddTransient<IIntegrationEventFinder, IntegrationEventFinder>();
                services.AddTransient<IIntegrationEventMapper, IntegrationEventMapper>();
                services.AddTransient<IOverviewOutputter, FileOutputter>();
            })
            .ConfigureLogging((_, logging) => 
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options => options.IncludeScopes = true);
            });

    
}

// ReSharper disable once ClassNeverInstantiated.Global
public class Options
{
    [Option('s', "solution", Required = true, HelpText = "Path to the solution file")]
    public string? SolutionPath { get; set; }
}