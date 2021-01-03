namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Models.Entity.Slot;

    public static class AggregateFactory
    {
        private const int InitialVersion = 0;

        public static IInventory StartNew(Guid nintendoUserId)
        {
            var newInventory = new InventoryAggregate(nintendoUserId, InitialVersion);

            newInventory.ApplyEvent(new InventoryCreated(newInventory.NintendoUserId));

            return newInventory;
        }

        public class InventoryBuilder
        {
            private readonly Guid nintendoUserId;
            private readonly int inventoryMajorVersion;
            private ISingleSlot<Shield> shieldSlot;
            private ISingleSlot<Weapon> weaponSlot;
            private IStackSlot<Material> materialSlot;

            public InventoryBuilder(Guid nintendoUserId)
            {
                this.nintendoUserId = nintendoUserId;
            }

            public InventoryBuilder(Guid nintendoUserId, int inventoryMajorVersion)
                : this(nintendoUserId)
            {
                this.inventoryMajorVersion = inventoryMajorVersion;
            }

            public InventoryBuilder WithManyWeapons(IEnumerable<Weapon> weapons)
            {
                this.weaponSlot = new WeaponSlot(weapons);

                return this;
            }

            public InventoryBuilder WithManyShields(IEnumerable<Shield> shields)
            {
                this.shieldSlot = new ShieldSlot(shields);

                return this;
            }

            public InventoryBuilder WithManyMaterials(IEnumerable<Material> materials)
            {
                this.materialSlot = new MaterialSlot(materials);

                return this;
            }

            public IInventory Build()
            {
                var inventory = new InventoryAggregate(
                        this.nintendoUserId,
                        this.inventoryMajorVersion,
                        this.weaponSlot,
                        this.shieldSlot,
                        this.materialSlot);

                return inventory;
            }
        }

        public class HistoryBuilder
        {
            private readonly List<InventoryDomainEvent> historyChanges = new List<InventoryDomainEvent>();

            private InventoryAggregate inventory;

            public HistoryBuilder(Guid nintendoUserId) { }

            public HistoryBuilder LoadEvents(IEnumerable<InventoryDomainEvent> events)
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
