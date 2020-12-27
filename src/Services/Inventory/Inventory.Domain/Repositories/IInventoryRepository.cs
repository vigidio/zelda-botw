namespace Inventory.Domain.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Inventory.Domain.Models.AggregateRoot;

    public interface IInventoryRepository : IAggregateRepository<IInventory>, IRepository<IInventory>
    {
        Task DeleteAsync(Guid nintendoUserId);
    }
}
