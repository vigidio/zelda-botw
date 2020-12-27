namespace Inventory.Domain.Models.Entity.Slot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Inventory.Domain.Exceptions;

    internal class MaterialSlot : IStackSlot<Material>
    {
        internal MaterialSlot(IEnumerable<Material> materials)
        {
            this.TotalSize = 160;
            this.TotalQuantities = 999;

            if (materials != null)
            {
                foreach (var material in materials)
                {
                    this.Add(material);
                }
            }
        }

        public IDictionary<Guid, ImmutableStack<Material>> SlotBag { get; private set; } = new Dictionary<Guid, ImmutableStack<Material>>();

        public int TotalSize { get; }

        public int TotalQuantities { get; }

        public void Add(Material item)
        {
            if (!this.SlotBag.ContainsKey(item.Id))
            {
                if (this.SlotBag.Count < this.TotalSize)
                {
                    this.AddMaterial(item);
                }
                else
                {
                    throw new FullSlotException(typeof(Material));
                }
            }
            else
            {
                this.IncrementQuantity(item);
            }
        }

        public void Remove(Guid materialId)
        {
            this.SlotBag[materialId] = this.SlotBag[materialId].Pop();

            if (!this.SlotBag[materialId].Any())
            {
                this.SlotBag.Remove(materialId);
            }
        }

        private void AddMaterial(Material material)
        {
            var stackMaterial = ImmutableStack.Create(material);

            this.SlotBag.Add(material.Id, stackMaterial);
        }

        private void IncrementQuantity(Material material)
        {
            if (this.SlotBag[material.Id].Count() == this.TotalQuantities)
            {
                throw new FullSlotException(typeof(Material));
            }

            this.SlotBag[material.Id] = this.SlotBag[material.Id].Push(material);
        }
    }
}
