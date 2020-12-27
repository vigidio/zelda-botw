namespace Inventory.Domain.Models.Entity
{
    using System;

    public class Weapon : IItem
    {
        public Weapon(
            Guid? id,
            string name,
            string description,
            int strength,
            int durability,
            string material,
            string archetype,
            string hands)
        {
            this.Id = id ?? Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.Strength = strength;
            this.Durability = durability;
            this.Material = material;
            this.Archetype = archetype;
            this.Hands = hands;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Description { get; }

        public int Strength { get; }

        public int Durability { get; }

        public string Material { get; } // TODO: Maybe Enum - wood, bone, stone, metal, and Ancient Technology

        public string Archetype { get; } // TODO: Maybe Enum - Korok Gear, Royal Gear, Gerudo Gear

        public string Hands { get; } // TODO: Maybe Enum - One-Handed, Two-Handed
    }
}
