namespace Inventory.Infra.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Configurations;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;

    /// <summary>
    ///     InventoryRepository class.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<IAggregateRoot> inventoryCollection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InventoryRepository" /> class.
        /// </summary>
        public InventoryRepository(RepositoryConfiguration configuration)
        {
            BsonClassMap.RegisterClassMap<AggregateRoot>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.InventoryIdentifier);
            });

            BsonSerializer.RegisterSerializer(
                new ImpliedImplementationInterfaceSerializer<IAggregateRoot, InventoryAggregate>(
                    BsonSerializer.LookupSerializer<InventoryAggregate>()));

            var client = new MongoClient(configuration.Mongo.ConnectionString);
            var database = client.GetDatabase("botw");
            inventoryCollection = database.GetCollection<IAggregateRoot>("inventory");
        }

        public Task SaveAsync(IAggregateRoot aggregate)
        {
            return inventoryCollection.InsertOneAsync(aggregate);
        }

        public async Task<IInventory> GetByIdAsync(string id)
        {
            return await inventoryCollection
                .Find(inv => inv.InventoryIdentifier == id)
                .SingleOrDefaultAsync() as IInventory;
        }

        public Task DeleteAsync(Guid nintendoUserId)
        {
            return Task.CompletedTask;
        }
    }
}