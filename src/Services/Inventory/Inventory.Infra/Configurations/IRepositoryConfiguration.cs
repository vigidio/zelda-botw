namespace Inventory.Infra.Configurations
{
    public interface IRepositoryConfiguration
    {
        public Mongo Mongo { get; }
    }
}