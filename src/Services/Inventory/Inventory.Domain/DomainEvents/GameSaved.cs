namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class GameSaved : Event
    {
        public GameSaved(Guid nintendoUserId, int currentVersion)
            : base(nintendoUserId, currentVersion)
        {
        }
    }
}
