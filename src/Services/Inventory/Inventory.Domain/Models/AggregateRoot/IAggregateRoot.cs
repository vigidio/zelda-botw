namespace Inventory.Domain.Models.AggregateRoot
{
    public interface IAggregateRoot
    {
        public string InventoryIdentifier { get; set; }

        public int EventVersion { get; }
    }
}
