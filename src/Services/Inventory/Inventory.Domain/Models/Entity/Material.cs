namespace Inventory.Domain.Models.Entity
{
    using System;

    public class Material : IItem
    {
        public Material(
            Guid? id,
            string name,
            string description,
            int hp,
            int time,
            MaterialType type)
        {
            this.Id = id ?? Guid.NewGuid();
            this.Name = name;
            this.Description = description;
            this.HP = hp;
            this.Time = time;
            this.Type = type;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Description { get; }

        public int HP { get; }

        public int Time { get; }

        public MaterialType Type { get; }
    }
}
