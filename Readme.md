# Event Overview Generator
This project is a tool to generate an overview of all integration and/or Domain events in a given solution. 

## Conventions
The tool will generate an overview for:
- Integration events that are defined in a class that implements an interface named `IIntegrationEvent` which in turn should inherit the MediatR (https://github.com/jbogard/MediatR) `INotification` interface.
If you don't follow this convention, you can specify the name of the interface the program should look for by using the `-i` or `--integration-event-name` option.
- Integration event handlers that are defined in a class that implements an interface named `IIntegrationEventListener` which in turn should inherit the MediatR `INotificationHandler<T>` interface, where `T` is the event type.
If you don't follow this convention, you can specify the name of the interface the program should look for by using the `-h` or `--integration-handler-name` option.

- domain events that are defined in a class that implements an interface named `IDomainEvent` which in turn should inherit the MediatR `INotification` interface.
If you don't follow this convention, you can specify the name of the interface the program should look for by using the `-d` or `--domain-event-name` option.
- domain event handlers that are defined in a class that implements an interface named `IDomainEventListener` which in turn should inherit the MediatR `INotificationHandler<T>` interface, where `T` is the event type.
  If you don't follow this convention, you can specify the name of the interface the program should look for by using the `-e` or `--domain-handler-name` option.

## Usage
1. Clone the Repository and build the project.

2. Run the program; it will need an option named "-s" or "--solution" to specify the absolute path for the solution to generate the overview for. Leave out any options for help.

3. After completion, the program will generate a file named "{Solution}-eventOverview.puml" in directory where you ran the program from.

### Output explanation
When rendered, the puml file will show these images:
   1. Integration Event overview
   2. Domain Event Overview: Highly connected (>2)
   3. Domain Event Overview: Less connected (1-2)
   4. Domain Event Overview: Lonely (0)

The Integration events are often fewer and the overview thus simpler. Therefore, the assumption is that these can be shown in one view without too much confusion

As for Domain events, there are often a lot of these, and we can help ourselves by only showing part of the view per image. Connectivity is defined by the number of handlers a certain event has.

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
