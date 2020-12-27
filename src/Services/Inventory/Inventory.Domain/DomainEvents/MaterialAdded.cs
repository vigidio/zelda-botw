namespace Inventory.Domain.DomainEvents
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class MaterialAdded : Event
    {
        public MaterialAdded(string inventoryIdentifier, int currentVersion, Guid itemId,
            string name, string description, int materialHp, int materialTime, string type)
            : base(inventoryIdentifier, currentVersion)
        {
            this.ItemId = itemId;
            this.Name = name;
            this.Description = description;
            this.HP = materialHp;
            this.Time = materialTime;
            this.Type = type;
        }

        [DataMember]
        public string Name { get; }

        [DataMember]
        public string Description { get; }

        [DataMember]
        public Guid ItemId { get; }

        [DataMember]
        public int HP { get; }

        [DataMember]
        public int Time { get; }

        [DataMember]
        public string Type { get; }
    }
}
