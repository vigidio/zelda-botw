namespace Inventory.Domain.CommandHandlers
{
    using System.Threading.Tasks;
    using Inventory.Domain.Commands;
    using Inventory.Domain.Models.AggregateRoot;

    public interface ICommandHandler<in T>
        where T : InventoryCommand
    {
        Task<IAggregateChanges> Handle(T message);
    }
}