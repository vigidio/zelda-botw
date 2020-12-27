namespace Inventory.Domain.CommandHandlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Inventory.Domain.Commands;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Exceptions;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;

    public class InventoryCommandHandler :
        ICommandHandler<NewGameCommand>,
        ICommandHandler<AddItemCommand>,
        ICommandHandler<RemoveItemCommand>,
        ICommandHandler<SaveCommand>
    {
        private readonly IDispatcherEvent dispatcherEvent;

        private readonly IInventoryRepository eventStoreRepository;

        private readonly IItemRepository itemRepository;

        public InventoryCommandHandler(
            IItemRepository itemRepository,
            IInventoryRepository eventStoreRepository,
            IDispatcherEvent dispatcherEvent)
        {
            this.itemRepository = itemRepository ?? throw new ArgumentNullException($"{nameof(itemRepository)} cannot be null");
            this.eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException($"{nameof(eventStoreRepository)} cannot be null");
            this.dispatcherEvent = dispatcherEvent ?? throw new ArgumentNullException($"{nameof(dispatcherEvent)} cannot be null");
        }

        public async Task<IAggregateChanges> Handle(NewGameCommand message)
        {
            var newInventory = AggregateFactory.StartNew(message.NintendoUserId);

            await this.eventStoreRepository.DeleteAsync(message.NintendoUserId);

            await this.eventStoreRepository.SaveAsync(newInventory);

            _ = this.dispatcherEvent.Send(newInventory.GetUncommitted().Last(o => o is InventoryCreated));

            return newInventory;
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

        public async Task<IAggregateChanges> Handle(SaveCommand message)
        {
            var inventoryIdentifier = $"{message.NintendoUserId}-{message.CurrentVersion}";

            var inventory = await this.eventStoreRepository.GetByIdAsync(inventoryIdentifier);

            if (inventory == null)
            {
                throw new InvalidInventoryException(message.NintendoUserId, message.CurrentVersion);
            }

            inventory.Save();

            await this.eventStoreRepository.SaveAsync(inventory);

            inventory.MarkChangesAsCommitted();

            return inventory;
        }
    }
}
