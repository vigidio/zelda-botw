namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class MaterialRemoved : InventoryDomainEvent
    {
        public MaterialRemoved(Guid nintendoUserId, int currentMajorVersion, Guid itemId)
            : base(nintendoUserId, currentMajorVersion)
        {
            this.ItemId = itemId;
        }

        [DataMember]
        public Guid ItemId { get; }
    }
}
