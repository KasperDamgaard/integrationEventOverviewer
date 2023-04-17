using MediatR;

namespace BuildingBlocks;

public interface IDomainEventListener<in T> : INotificationHandler<T> where T : IDomainEvent 
{
    
}