namespace Inventory.Domain.DomainEvents
{
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class Event
    {
        protected Event(string inventoryIdentifier)
        {
            this.InventoryIdentifier = inventoryIdentifier;
        }

        protected Event(string inventoryIdentifier, int currentMajorVersion)
        {
            this.InventoryIdentifier = inventoryIdentifier;
            this.MajorVersion = currentMajorVersion;
        }

        [DataMember]
        public int Version;

        [DataMember]
        public string InventoryIdentifier { get; }

        [DataMember]
        public int MajorVersion { get; }
    }
}
