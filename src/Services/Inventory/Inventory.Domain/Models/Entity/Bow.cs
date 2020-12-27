namespace Inventory.Domain.Models.Entity
{
    using System;

    public class Bow : IItem
    {
        public Guid Id { get; }

        public string Name { get; }

        public string Description { get; }
    }
}
