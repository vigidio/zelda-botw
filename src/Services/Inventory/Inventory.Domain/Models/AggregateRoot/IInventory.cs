namespace Inventory.Domain.Models.AggregateRoot
{
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Models.Entity.Slot;

    public interface IInventory : IAggregateRoot, IAggregateChanges
    {
        ISingleSlot<Weapon> WeaponSlot { get; }

        ISingleSlot<Shield> ShieldSlot { get; }

        IStackSlot<Material> MaterialSlot { get; }
        
        int TotalItems { get; }

        IInventory AddItem(IItem item);

        IInventory RemoveItem(IItem item);

        IInventory Save();
    }
}
