// See https://aka.ms/new-console-template for more information

using CommandLine;
using IntegrationEventOverview.Output;
using IntegrationEventOverview.Visualization;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationEventOverview;

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
        var options = Parser.Default.ParseArguments<CliOptions>(args).Value
                      ?? throw new ArgumentException("No options provided");
        var workerInstance = host.Services.GetRequiredService<OverviewComputer>();
        await workerInstance.ComputeOverview(options);
        await host.StopAsync();
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                var options = Parser.Default.ParseArguments<CliOptions>(args).Value
                    ?? throw new ArgumentException("No options provided");
                
                services.AddTransient<OverviewComputer>();
                services.AddTransient<IVisualizer, PumlVisualizer>();
                services.AddTransient<IEventFinder, EventFinder>();
                services.AddTransient<IEventMapper, EventMapper>();
                
                switch (options.OutputType.ToLower())
                {
                    case "file":
                        services.AddTransient<IOverviewOutputter, FileOutputter>();
                        break;
                    case "console":
                        services.AddTransient<IOverviewOutputter, ConsoleOutputter>();
                        break;
                    default:
                        throw new ArgumentException("Invalid output type", nameof(options.OutputType));
                }
                
                services.Configure<SolutionOptions>(o =>
                {
                    o.SolutionPath = options.SolutionPath!;
                    o.IntegrationEventInterfaceName = options.IntegrationEventInterfaceName;
                    o.IntegrationEventHandlerInterfaceName = options.IntegrationEventHandlerInterfaceName;
                    o.DomainEventOverview = options.DomainEventOverview;
                });
            })
            .ConfigureLogging((_, logging) => 
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options => options.IncludeScopes = true);
            });

    
}

// ReSharper disable once ClassNeverInstantiated.Global
public class CliOptions
{
    [Option('s', "solution", Required = true, HelpText = "Path to the solution file")]
    public string? SolutionPath { get; set; }

    [Option('t', "output-type", Required = false, Default = "File", HelpText = "The output type (File or Console)")]
    public string OutputType { get; set; } = "File";
    
    [Option('h', "handler-name", Required = false, HelpText = "The name of the interface which handles the Integration Event")]
    public string? IntegrationEventHandlerInterfaceName { get; set; }
    
    [Option('i', "integration-event-name", Required = false, HelpText = "The name of the Integration Event interface class")]
    public string? IntegrationEventInterfaceName { get; set; }
    [Option('d', "domain-events", Required = false, HelpText = "If set, the output will be the domain events instead of the integration events")]
    public bool DomainEventOverview { get; set; }
}