namespace Inventory.Domain.Exceptions
{
    using System;
    using System.Diagnostics;

    public class InvalidInventoryException : Exception
    {
        public InvalidInventoryException(Guid nintendoUserId, int version)
        {
            Trace.WriteLine($"Invalid Inventory for this user {nintendoUserId} and version {version}");
        }
    }
}
