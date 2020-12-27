namespace Inventory.Domain.Exceptions
{
    using System;
    using System.Diagnostics;

    public class InvalidItemException : Exception
    {
        public InvalidItemException(Guid itemId)
        {
            Debug.WriteLine($"Invalid ItemId: {itemId}");
        }
    }
}
