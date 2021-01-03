namespace Inventory.Domain.DomainEvents
{
    using System;

    public interface IInventoryDomainEvent
    {
        Guid NintendoUserId { get; }
        
        int Version { get; }
    }
}
