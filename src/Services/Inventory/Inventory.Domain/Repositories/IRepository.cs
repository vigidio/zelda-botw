namespace Inventory.Domain.Repositories
{
    using System.Threading.Tasks;

    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(string id);
    }
}
