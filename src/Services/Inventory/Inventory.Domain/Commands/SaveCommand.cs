namespace Inventory.Domain.Commands
{
    using System;

    public class SaveCommand : InventoryCommand
    {
        public SaveCommand(Guid nintendoUserId, int currentVersion)
        {
            this.NintendoUserId = nintendoUserId;
            this.CurrentVersion = currentVersion;
        }

        public Guid NintendoUserId { get; }

        public int CurrentVersion { get; }
    }
}
