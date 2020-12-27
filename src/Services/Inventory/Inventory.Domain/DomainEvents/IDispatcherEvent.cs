namespace Inventory.Domain.DomainEvents
{
    using System.Threading.Tasks;

    public interface IDispatcherEvent
    {
        Task Send(Event @event);
    }
}