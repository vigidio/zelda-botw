namespace Inventory.Domain.UseCases.RemoveItem
{
    using System.Threading.Tasks;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Commands;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Exceptions;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;

    public class RemoveItemCommandHandler : ICommandHandler<RemoveItemCommand>
    {
        private readonly IInventoryRepository eventStoreRepository;

        private readonly IItemRepository itemRepository;

        public RemoveItemCommandHandler(    IItemRepository itemRepository,
            IInventoryRepository eventStoreRepository,
            IDispatcherEvent dispatcherEvent)
        {
            this.itemRepository = itemRepository;
            this.eventStoreRepository = eventStoreRepository;
        }
        
        public async Task<IAggregateChanges> Handle(RemoveItemCommand message)
        {
            var item = await this.itemRepository.GetByIdAsync(
                message.ItemId.ToString());

            if (item == null)
            {
                throw new InvalidItemException(message.ItemId);
            }

            var inventory = await this.eventStoreRepository.GetByIdAsync(message.InventoryIdentifier);

            if (inventory == null)
            {
                throw new InvalidInventoryException(message.InventoryIdentifier);
            }

            inventory.RemoveItem(item);

            _ = this.eventStoreRepository.SaveAsync(inventory); //TODO: Fire and forget but with log for exceptions

            return inventory;
        }
    }
}