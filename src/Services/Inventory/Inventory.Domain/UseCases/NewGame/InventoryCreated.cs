namespace Inventory.Domain.UseCases.NewGame
{
    using System.Runtime.Serialization;
    using Inventory.Domain.DomainEvents;

    [DataContract]
    public class InventoryCreated : Event
    {
        public InventoryCreated(string inventoryIdentifier)
            : base(inventoryIdentifier)
        {
        }
    }
}
