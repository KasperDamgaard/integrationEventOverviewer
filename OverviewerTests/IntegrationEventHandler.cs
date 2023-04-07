using IntegrationEventOverview;
using MediatR;

namespace OverviewerTests;

public class IntegrationEventHandler : INotificationHandler<IntegrationEventTestImplementor>
{
    public Task Handle(IntegrationEventTestImplementor notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class WrongIntegrationEventTestImplementor : IIntegrationEvent
{
    
}

public class WrongIntegrationEventHandler : INotificationHandler<WrongIntegrationEventTestImplementor>
{
    public Task Handle(WrongIntegrationEventTestImplementor notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}