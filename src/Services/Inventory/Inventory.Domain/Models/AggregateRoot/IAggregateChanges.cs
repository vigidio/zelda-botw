namespace Inventory.Domain.Models.AggregateRoot
{
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;

    public interface IAggregateChanges
    {
        int MajorVersion { get; }

        IEnumerable<Event> GetUncommitted();

        IEnumerable<Event> GetLastCommitted();

        void MarkChangesAsCommitted();
    }
}
