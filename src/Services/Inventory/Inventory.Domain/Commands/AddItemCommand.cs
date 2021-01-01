namespace Inventory.Domain.Commands
{
    using System;
    using Inventory.Domain.Models.Entity;

    public class AddItemCommand : InventoryCommand
    {
        public AddItemCommand(Guid nintendoUserId, int version, Guid itemId, ItemType itemType)
        {
            this.NintendoUserId = nintendoUserId;
            this.Version = version;
            this.ItemId = itemId;
        }

        public Guid NintendoUserId { get; }

        public int Version { get; }

        public Guid ItemId { get; }
    }
}
