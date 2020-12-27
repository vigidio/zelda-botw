namespace Inventory.Domain.Exceptions
{
    using System;
    using System.Diagnostics;

    public class FullSlotException : Exception
    {
        public FullSlotException(Type type)
        {
            Debug.WriteLine($"Cannot add {type} because slots are allocated");
        }
    }
}
