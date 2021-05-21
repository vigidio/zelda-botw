namespace Inventory.Domain.UseCases.RemoveItem
{
    using System;
    using System.Runtime.Serialization;
    using Inventory.Domain.DomainEvents;

    [DataContract]
    public class ShieldRemoved : Event
    {
        public ShieldRemoved(string inventoryIdentifier, int currentMajorVersion, Guid itemId) 
            : base(inventoryIdentifier, currentMajorVersion)
        {
            this.ItemId = itemId;
        }
        
        [DataMember]
        public Guid ItemId { get; }
    }
}