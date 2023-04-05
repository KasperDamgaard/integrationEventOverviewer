// See https://aka.ms/new-console-template for more information

using IntegrationEventOverviewer;
using Microsoft.Build.Locator;

var eventFinder = new IntegrationEventFinder();
var implementors = await eventFinder.FindIntegrationEventImplementors(args[0]);
Console.WriteLine("Found these classes: " + implementors.Aggregate("", (current, @event) => current + @event + Environment.NewLine));


