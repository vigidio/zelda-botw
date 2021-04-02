namespace Inventory.Domain.UseCases.SaveGame
{
    using System.Threading.Tasks;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Exceptions;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;

    public class SaveGameCommandHandler : ICommandHandler<SaveGameCommand>
    {
        private readonly IInventoryRepository eventStoreRepository;

        public SaveGameCommandHandler(IInventoryRepository eventStoreRepository)
        {
            this.eventStoreRepository = eventStoreRepository;
        }
        
        public async Task<IAggregateChanges> Handle(SaveGameCommand message)
        {
            string inventoryIdentifier = $"{message.NintendoUserId}-{message.CurrentVersion}";

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