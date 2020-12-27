namespace Inventory.Domain.Models.AggregateRoot
{
    public interface IAggregateRoot
    {
        public string InventoryIdentifier { get; }

        public int EventVersion { get; }
    }
}
