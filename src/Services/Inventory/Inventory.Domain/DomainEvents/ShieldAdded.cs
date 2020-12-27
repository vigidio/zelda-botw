namespace Inventory.Domain.DomainEvents
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ShieldAdded : Event
    {
        public ShieldAdded(string inventoryIdentifier, int currentVersion)
            : base(inventoryIdentifier, currentVersion) { }
    }
}
