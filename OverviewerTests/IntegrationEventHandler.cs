using MediatR;

namespace OverviewerTests;

public class IntegrationEventListener : BuildingBlocks.IIntegrationEventListener<IntegrationEventTestImplementor2>
{
    public Task Handle(IntegrationEventTestImplementor2 notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class IntegrationEventHandler : INotificationHandler<IntegrationEventTestImplementor>
{
    public Task Handle(IntegrationEventTestImplementor notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class WrongIntegrationEventTestImplementor : IntegrationEventOverview.IIntegrationEvent
{
    
}

public class WrongIntegrationEventHandler : INotificationHandler<WrongIntegrationEventTestImplementor>
{
    public Task Handle(WrongIntegrationEventTestImplementor notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}