namespace Inventory.Domain.Commands
{
    using System;

    public class NewGameCommand : InventoryCommand
    {
        public NewGameCommand(Guid nintendoUserId)
        {
            this.NintendoUserId = nintendoUserId;
        }

        public Guid NintendoUserId { get; }
    }
}
