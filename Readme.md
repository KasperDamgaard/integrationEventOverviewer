# Integration Event Overview Generator
This project is a tool to generate an overview of all integration events in a given solution. 

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
