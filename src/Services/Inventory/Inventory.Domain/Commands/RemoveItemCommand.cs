namespace Inventory.Domain.Commands
{
    using System;
    using Inventory.Domain.Models.Entity;

    public class RemoveItemCommand : InventoryCommand
    {
        public RemoveItemCommand(string inventoryIdentifier, Guid itemId, ItemType itemType)
        {
            this.InventoryIdentifier = inventoryIdentifier;
            this.ItemId = itemId;
        }

        public string InventoryIdentifier { get; }

        public Guid ItemId { get; }
    }
}
