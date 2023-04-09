using MediatR;

namespace BuildingBlocks;

public interface IIntegrationEventListener<T> : INotificationHandler<T> where T : IIntegrationEvent
{
}