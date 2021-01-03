namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class InventoryDomainEvent : IInventoryDomainEvent
    {
        protected InventoryDomainEvent(Guid nintendoUserId)
        {
            this.NintendoUserId = nintendoUserId;
        }

        protected InventoryDomainEvent(Guid nintendoUserId, int currentMajorVersion)
        {
            this.NintendoUserId = nintendoUserId;
            this.Version = currentMajorVersion;
        }

        [DataMember]
        public Guid NintendoUserId { get; }

        [DataMember]
        public int Version { get; }
    }
}