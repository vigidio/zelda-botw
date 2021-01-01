namespace Inventory.Infra.Configurations
{
    public class RepositoryConfiguration : IRepositoryConfiguration
    {
        public Mongo Mongo { get; set; }
    }
}