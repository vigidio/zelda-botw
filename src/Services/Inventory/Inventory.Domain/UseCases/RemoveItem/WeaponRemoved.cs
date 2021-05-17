namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WeaponRemoved : Event
    {
        public WeaponRemoved(string inventoryIdentifier, int currentMajorVersion, Guid itemId)
            : base(inventoryIdentifier, currentMajorVersion)
        {
            this.ItemId = itemId;
        }

        [DataMember]
        public Guid ItemId { get; }
    }
}
