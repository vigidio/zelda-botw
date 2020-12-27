namespace Inventory.Domain.Models.Entity.Slot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Inventory.Domain.Exceptions;

    internal class ShieldSlot : ISingleSlot<Shield>
    {
        internal ShieldSlot(IEnumerable<Shield> shields)
        {
            this.TotalSize = 4;

            if (shields != null)
            {
                foreach (var shield in shields)
                {
                    this.Add(shield);
                }
            }
        }

        public ImmutableList<Shield> SlotBag { get; private set; } = ImmutableList<Shield>.Empty;

        public int TotalSize { get; }

        public void Add(Shield shield)
        {
            if (this.SlotBag.Count < this.TotalSize)
            {
                this.SlotBag = this.SlotBag.Add(shield);
            }
            else
            {
                throw new FullSlotException(typeof(Shield));
            }
        }

        public void Remove(Guid itemId)
        {
            throw new NotImplementedException();
        }
    }
}
