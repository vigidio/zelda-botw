namespace Inventory.Domain.Models.Entity.Slot
{
    using System.Collections.Immutable;

    public interface ISingleSlot<T> : ISlot<T>
    {
        ImmutableList<T> SlotBag { get; }
    }
}
