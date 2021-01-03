namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;

    public interface IAggregateChanges : IAggregateRoot
    {
        IEnumerable<InventoryDomainEvent> GetUncommitted();

        IEnumerable<InventoryDomainEvent> GetLastCommitted();

        void MarkChangesAsCommitted();
    }
}
