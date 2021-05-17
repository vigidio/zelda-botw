namespace Inventory.Domain.Models.Entity.Slot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Inventory.Domain.Exceptions;

    internal class WeaponSlot : ISingleSlot<Weapon>
    {
        private const int InitialSize = 8;

        internal WeaponSlot(IEnumerable<Weapon> weapons)
        {
            if (weapons != null)
            {
                foreach (var weapon in weapons)
                {
                    this.Add(weapon);
                }
            }
        }

        public ImmutableList<Weapon> SlotBag { get; private set; } = ImmutableList<Weapon>.Empty;

        public int TotalSize => InitialSize;

        public void Add(Weapon weapon)
        {
            if (this.SlotBag.Count < this.TotalSize)
            {
                this.SlotBag = this.SlotBag.Add(weapon);
            }
            else
            {
                throw new FullSlotException(typeof(Weapon));
            }
        }

        public void Remove(Guid itemId)
        {
            var weapon = this.SlotBag.FirstOrDefault(o => o.Id == itemId);

            if (weapon == null) return;
            
            var immutableList = this.SlotBag.Remove(weapon);
            this.SlotBag = immutableList;
        }
    }
}
