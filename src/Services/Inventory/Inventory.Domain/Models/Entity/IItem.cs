namespace Inventory.Domain.Models.Entity
{
    using System;

    public interface IItem
    {
        public Guid Id { get; }

        public string Name { get; }

        public string Description { get; }
    }
}
