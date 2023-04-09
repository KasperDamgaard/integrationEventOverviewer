using MediatR;

namespace IntegrationEventOverview;

public interface IIntegrationEventListener<in T> : INotificationHandler<T> where T : IIntegrationEvent 
{
    
}