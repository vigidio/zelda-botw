namespace Inventory.Domain.UseCases.NewGame
{
    using System;
    using Inventory.Domain.Commands;

    public class NewGameCommand : InventoryCommand
    {
        public NewGameCommand(Guid nintendoUserId)
        {
            this.NintendoUserId = nintendoUserId;
        }

        public Guid NintendoUserId { get; }
    }
}
