namespace Inventory.Infra.Repositories
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Configurations;
    using Microsoft.Extensions.Options;


    /// <summary>
    /// InventoryRepository class.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<IAggregateRoot> inventoryCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryRepository"/> class.
        /// </summary>
        public InventoryRepository(IOptions<RepositoryConfiguration> configuration)
        {
            BsonClassMap.RegisterClassMap<AggregateRoot>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.InventoryIdentifier);
            });

            var client = new MongoClient(configuration.Value.Mongo.ConnectionString);
            var database = client.GetDatabase("botw");
            this.inventoryCollection = database.GetCollection<IAggregateRoot>("inventory");
        }

        public Task SaveAsync(IAggregateRoot aggregate) => this.inventoryCollection.InsertOneAsync(aggregate);

        public async Task<IInventory> GetByIdAsync(string id) =>
            await this.inventoryCollection
                .Find(inv => inv.InventoryIdentifier == id)
                .SingleOrDefaultAsync() as InventoryAggregate;

        public Task DeleteAsync(Guid nintendoUserId) => Task.CompletedTask;
    }
}
