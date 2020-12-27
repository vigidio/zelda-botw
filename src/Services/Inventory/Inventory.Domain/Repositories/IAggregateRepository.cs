namespace Inventory.Domain.Repositories
{
    using System.Threading.Tasks;
    using Inventory.Domain.Models.AggregateRoot;

    public interface IAggregateRepository<T>
        where T : IAggregateRoot
    {
        Task SaveAsync(IAggregateRoot aggregate);
    }
}
