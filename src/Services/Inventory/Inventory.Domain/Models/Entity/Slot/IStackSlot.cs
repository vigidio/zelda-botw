namespace Inventory.Domain.Models.Entity.Slot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public interface IStackSlot<T> : ISlot<T>
    {
        IDictionary<Guid, ImmutableStack<T>> SlotBag { get; }

        int TotalQuantities { get; }
    }
}
