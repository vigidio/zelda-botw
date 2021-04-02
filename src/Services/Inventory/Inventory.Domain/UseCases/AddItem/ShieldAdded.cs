namespace Inventory.Domain.UseCases.AddItem
{
    using System.Runtime.Serialization;
    using Inventory.Domain.DomainEvents;

    [DataContract]
    public class ShieldAdded : Event
    {
        public ShieldAdded(string inventoryIdentifier, int currentVersion)
            : base(inventoryIdentifier, currentVersion) { }
    }
}
