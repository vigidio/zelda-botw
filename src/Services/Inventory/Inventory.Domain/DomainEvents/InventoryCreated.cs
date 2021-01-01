namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class InventoryCreated : Event
    {
        public InventoryCreated(Guid nintendoUserId)
            : base(nintendoUserId)
        {
        }
    }
}