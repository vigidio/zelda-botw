namespace Inventory.Domain.DomainEvents
{
    using System.Runtime.Serialization;

    [DataContract]
    public class InventoryCreated : Event
    {
        public InventoryCreated(string inventoryIdentifier)
            : base(inventoryIdentifier)
        {
        }
    }
}
