namespace Inventory.Domain.UseCases.AddItem
{
    using System.Threading.Tasks;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Exceptions;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;

    public class AddItemCommandHandler : ICommandHandler<AddItemCommand>
    {
        private readonly IInventoryRepository eventStoreRepository;

        private readonly IItemRepository itemRepository;

        public AddItemCommandHandler(
            IItemRepository itemRepository,
            IInventoryRepository eventStoreRepository)
        {
            this.itemRepository = itemRepository;
            this.eventStoreRepository = eventStoreRepository;
        }
        
        public async Task<IAggregateChanges> Handle(AddItemCommand message)
        {
            var item = await this.itemRepository.GetByIdAsync(message.ItemId.ToString());

            if (item == null)
            {
                throw new InvalidItemException(message.ItemId);
            }

            var inventory = await this.eventStoreRepository.GetByIdAsync(message.InventoryIdentifier);

            if (inventory == null)
            {
                throw new InvalidInventoryException(message.InventoryIdentifier);
            }

            inventory.AddItem(item);

            _ = this.eventStoreRepository.SaveAsync(inventory); //TODO: Fire and forget but with log for exceptions

            return inventory;
        }
    }
}