namespace Inventory.Domain.Models.AggregateRoot
{
    using System;

    public interface IAggregateRoot
    {
        public Guid NintendoUserId { get; }

        public int MajorVersion { get; }
        
        public int EventVersion { get; }
    }
}
