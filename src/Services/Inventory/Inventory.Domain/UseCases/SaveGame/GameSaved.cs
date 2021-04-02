namespace Inventory.Domain.UseCases.SaveGame
{
    using System.Runtime.Serialization;
    using Inventory.Domain.DomainEvents;

    [DataContract]
    public class GameSaved : Event
    {
        public GameSaved(string inventoryIdentifier, int currentVersion)
            : base(inventoryIdentifier, currentVersion)
        {
        }
    }
}
