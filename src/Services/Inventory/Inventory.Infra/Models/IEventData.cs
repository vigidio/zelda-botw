namespace Inventory.Infra.Models
{
    using System;

    public interface IEventData
    {
        public Guid EventId { get; set; }
        public Guid NintendoUserId { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }
        public bool IsJson { get; set; }
        public byte[] Data { get; set; }
        public byte[] Metadata { get; set; }
    }
}