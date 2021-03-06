namespace Inventory.Domain.UseCases.AddItem
{
    using System;
    using System.Runtime.Serialization;
    using Inventory.Domain.DomainEvents;

    [DataContract]
    public class WeaponAdded : Event
    {
        public WeaponAdded(
            string inventoryIdentifier,
            int currentVersion,
            Guid itemId,
            string name,
            string description,
            int strength,
            int durability,
            string material,
            string archetype,
            string hands)
            : base(inventoryIdentifier, currentVersion)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.Description = description;
            this.Strength = strength;
            this.Durability = durability;
            this.Material = material;
            this.Archetype = archetype;
            this.Hands = hands;
        }

        [DataMember]
        public Guid ItemId { get; }

        [DataMember]
        public string Name { get; }

        [DataMember]
        public string Description { get; }

        [DataMember]
        public int Strength { get; }

        [DataMember]
        public int Durability { get; }

        [DataMember]
        public string Material { get; }

        [DataMember]
        public string Archetype { get; }

        [DataMember]
        public string Hands { get; }
    }
}
