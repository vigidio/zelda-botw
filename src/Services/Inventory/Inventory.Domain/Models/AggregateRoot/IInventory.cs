namespace Inventory.Domain.Models.AggregateRoot
{
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Models.Entity.Slot;

    public interface IInventory : IAggregateChanges
    {
        ISingleSlot<Weapon> WeaponSlot { get; }

        ISingleSlot<Shield> ShieldSlot { get; }

        IStackSlot<Material> MaterialSlot { get; }

        void AddItem(IItem item);

        void RemoveItem(IItem item);

        void Save();
    }
}
