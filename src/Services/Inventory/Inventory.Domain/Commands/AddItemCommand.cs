namespace Inventory.Domain.Commands
{
    using System;
    using Inventory.Domain.Models.Entity;

    public class AddItemCommand : InventoryCommand
    {
        public AddItemCommand(string inventoryIdentifier, Guid itemId, ItemType itemType)
        {
            this.InventoryIdentifier = inventoryIdentifier;
            this.ItemId = itemId;
        }

        public string InventoryIdentifier { get; }

        public Guid ItemId { get; }
    }
}
