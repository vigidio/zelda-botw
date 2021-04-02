namespace Inventory.Domain.UseCases.SaveGame
{
    using System;
    using Inventory.Domain.Commands;

    public class SaveGameCommand : InventoryCommand
    {
        public SaveGameCommand(Guid nintendoUserId, int currentVersion)
        {
            this.NintendoUserId = nintendoUserId;
            this.CurrentVersion = currentVersion;
        }

        public Guid NintendoUserId { get; }

        public int CurrentVersion { get; }
    }
}
