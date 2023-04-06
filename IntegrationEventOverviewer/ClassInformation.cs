namespace IntegrationEventOverviewer;


public record Namespace(string Name);
public record ClassInformation(string Name, Namespace Namespace);

public record IntegrationEventClassInformation(string Name, Namespace Namespace);

public record HandlerClassInformation(string Name, Namespace Namespace);