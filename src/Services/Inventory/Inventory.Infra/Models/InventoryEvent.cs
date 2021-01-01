namespace Inventory.Infra.Models
{
    using System;
    
    public class EventData : IEventData
    {
        public EventData(
            Guid eventId,
            Guid nintendoUserId,
            int version,
            string type, 
            bool isJson, 
            byte[] data, 
            byte[] metadata)
        {
            this.EventId = eventId;
            this.NintendoUserId = nintendoUserId;
            this.Version = version;
            this.Type = type;
            this.IsJson = isJson;
            this.Data = data ?? new byte[]{};
            this.Metadata = metadata ?? new byte[]{};
        }

        public Guid EventId { get; set; }
        public Guid NintendoUserId { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }
        public bool IsJson { get; set; }
        public byte[] Data { get; set; }
        public byte[] Metadata { get; set; }
    }
}