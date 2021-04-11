namespace Inventory.EventDispatcher
{
    using System.Threading.Tasks;
    using Inventory.Domain.DomainEvents;

    public class DispatcherEvent : IDispatcherEvent
    {
        public Task Send(Event @event)
        {
            return Task.CompletedTask;
        }
    }
}