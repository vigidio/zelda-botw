namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ShieldAdded : Event
    {
        public ShieldAdded(Guid nintendoUserId, int currentVersion)
            : base(nintendoUserId, currentVersion) { }
    }
}
