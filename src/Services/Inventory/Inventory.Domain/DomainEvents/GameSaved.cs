namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class GameSaved : InventoryDomainEvent
    {
        public GameSaved(Guid nintendoUserId, int currentVersion)
            : base(nintendoUserId, currentVersion)
        {
        }
    }
}
