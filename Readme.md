# Event Overview Generator
This project is a tool to generate an overview of all integration and/or Domain events in a given solution. 

## Conventions
The tool will by default only generate an overview for integration events that are defined in a class that implements an interface named `IIntegrationEvent` which in turn should inherit the MediatR (https://github.com/jbogard/MediatR) `INotification` interface.
Options exist to specify the name of the interface the program should look for. 

If the option `-d true` is specified, the tool will generate an overview for Domain events instead. Here, the convention is that classes should implement the interface named `IDomainEvent` which in turn should inherit the MediatR `INotification` interface.

Event handlers are expected to implement the MediatR `INotificationHandler<T>` interface where `T` is the event type.

## Usage
1. Clone the Repository and build the project.

2. Run the program; it will need an option named "-s" or "--solution" to specify the absolute path for the solution to generate the overview for. Leave out any options for help.

3. After completion, the program will generate a file named "{Solution}-IntegrationEventOverview.puml" in directory where you ran the program from.

### Example
``` bash
dotnet run -s "C:\MySolution.sln"
```

## Requirements
- .NET Core 7.0 or higher

### Optional (for viewing the generated file)
- PlantUML (https://plantuml.com/)
- Graphviz (https://graphviz.org/)
- PlantUML Visual Studio Code Extension (https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml)
- PlantUML IntelliJ plugin (https://plugins.jetbrains.com/plugin/7017-plantuml-integration)
