namespace Inventory.Domain.Repositories
{
    using System.Threading.Tasks;
    using Inventory.Domain.Models.Entity;

    public interface IItemRepository : IRepository<IItem>
    {
        Task<IItem> GetByIdAsync(string id);
    }
}
