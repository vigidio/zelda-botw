namespace Inventory.Domain.DomainEvents
{
    using System.Runtime.Serialization;

    [DataContract]
    public class GameSaved : Event
    {
        public GameSaved(string inventoryIdentifier, int currentVersion)
            : base(inventoryIdentifier, currentVersion)
        {
        }
    }
}
