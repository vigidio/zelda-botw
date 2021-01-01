namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class Event
    {
        protected Event(Guid nintendoUserId)
        {
            this.NintendoUserId = nintendoUserId;
        }

        protected Event(Guid nintendoUserId, int currentMajorVersion)
        {
            this.NintendoUserId = nintendoUserId;
            this.MajorVersion = currentMajorVersion;
        }

        [DataMember]
        public int Version;

        [DataMember]
        public Guid NintendoUserId { get; }

        [DataMember]
        public int MajorVersion { get; }
    }
}
