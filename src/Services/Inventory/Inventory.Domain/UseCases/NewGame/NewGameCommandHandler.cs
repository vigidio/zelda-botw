namespace Inventory.Domain.UseCases.NewGame
{
    using System.Linq;
    using System.Threading.Tasks;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;
    
    public class NewGameCommandHandler : ICommandHandler<NewGameCommand>
    {
        private readonly IDispatcherEvent dispatcherEvent;

        private readonly IInventoryRepository eventStoreRepository;

        public NewGameCommandHandler(
            IInventoryRepository eventStoreRepository,
            IDispatcherEvent dispatcherEvent)
        {
            this.eventStoreRepository = eventStoreRepository;
            this.dispatcherEvent = dispatcherEvent;            
        }
        public async Task<IAggregateChanges> Handle(NewGameCommand message)
        {
            var newInventory = AggregateFactory.StartNew(message.NintendoUserId);

            await this.eventStoreRepository.DeleteAsync(message.NintendoUserId);

            await this.eventStoreRepository.SaveAsync(newInventory);

            _ = this.dispatcherEvent.Send(newInventory.GetUncommitted().Last(o => o is InventoryCreated));

            return newInventory;
        }
    }
}