namespace Inventory.Domain.Models.Entity.Slot
{
    using System;

    public interface ISlot<T>
    {
        int TotalSize { get; }

        void Add(T item);

        void Remove(Guid itemId);
    }
}
