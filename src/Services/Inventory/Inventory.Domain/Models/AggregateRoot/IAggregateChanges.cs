namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;

    public interface IAggregateChanges
    {
        Guid NintendoUserId { get; }
        
        int MajorVersion { get; }

        IEnumerable<Event> GetUncommitted();

        IEnumerable<Event> GetLastCommitted();

        void MarkChangesAsCommitted();
    }
}
