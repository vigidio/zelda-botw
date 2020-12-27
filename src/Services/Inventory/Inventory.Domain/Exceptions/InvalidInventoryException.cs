namespace Inventory.Domain.Exceptions
{
    using System;
    using System.Diagnostics;

    public class InvalidInventoryException : Exception
    {
        public InvalidInventoryException(string inventoryIdentifier)
        {
            Trace.WriteLine($"Invalid Inventory: {inventoryIdentifier}");
        }

        public InvalidInventoryException(Guid nintendoUserId, int version)
        {
            Trace.WriteLine($"Invalid Inventory for this user {nintendoUserId} and version {version}");
        }
    }
}
