namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Models.Entity.Slot;
    using Inventory.Domain.UseCases.NewGame;

    public static class InventoryFactory
    {
        private const int InitialVersion = 0;

        public static IInventory Create(Guid nintendoUserId)
        {
            var newInventory = new InventoryAggregate(nintendoUserId, InitialVersion);

            newInventory.ApplyEvent(new InventoryCreated(newInventory.InventoryIdentifier));

            return newInventory;
        }
        
        public class HistoryBuilder
        {
            private readonly List<Event> historyChanges = new List<Event>();

            private InventoryAggregate inventory;

            public HistoryBuilder(Guid nintendoUserId) { }

            public HistoryBuilder LoadEvents(IEnumerable<Event> events)
            {
                this.historyChanges.AddRange(events);

                return this;
            }

            public IInventory Build()
            {
                this.inventory = new InventoryAggregate();

                this.inventory.LoadsFromHistory(this.historyChanges);

                return this.inventory;
            }
        }
    }
}
