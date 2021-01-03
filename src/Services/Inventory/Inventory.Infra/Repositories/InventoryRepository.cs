namespace Inventory.Infra.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Configurations;
    using Inventory.Infra.Models;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;
    using Newtonsoft.Json;

    /// <summary>
    ///     InventoryRepository class.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<IEventData> inventoryCollection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InventoryRepository" /> class.
        /// </summary>
        public InventoryRepository(IMongoCollection<IEventData> inventoryCollection)
        {
            this.inventoryCollection = inventoryCollection;
            // if (!BsonClassMap.IsClassMapRegistered(typeof(AggregateRoot)))
            // {
            //     BsonClassMap.RegisterClassMap<AggregateRoot>(cm =>
            //     {
            //         cm.AutoMap();
            //         cm.MapIdProperty(c => c.NintendoUserId);
            //         cm.MapIdProperty(c => c.MajorVersion);
            //     });
            //
            //     BsonSerializer.RegisterSerializer(
            //         new ImpliedImplementationInterfaceSerializer<IAggregateChanges, InventoryAggregate>(
            //             BsonSerializer.LookupSerializer<InventoryAggregate>()));
            // }
            //
            // var client = new MongoClient(configuration.Mongo.ConnectionString);
            // var database = client.GetDatabase("botw");
            // inventoryCollection = database.GetCollection<IAggregateChanges>("inventory");
        }

        public Task SaveAsync(IAggregateChanges aggregate)
        {
            var uncommittedChanges = aggregate.GetUncommitted()
                .Select(@event => new EventData(
                    Guid.NewGuid(), 
                    @event.NintendoUserId, 
                    @event.Version, 
                    @event.GetType().Name,
                    true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)), 
                        null))
                .ToArray();
            
            return this.inventoryCollection.InsertManyAsync(uncommittedChanges);
        }

        public async Task<IInventory> GetByIdAsync(Guid nintendoUserId, int version)
        {
            var @events = await this.inventoryCollection
                .FindAsync(inv => 
                    inv.NintendoUserId == nintendoUserId && inv.Version == version);

            var aggregate = new AggregateFactory.HistoryBuilder(nintendoUserId);
            var loadedEvents = await @events.ToListAsync();
            var inventoryDomainEvents = new List<InventoryDomainEvent>();
            foreach (var loadedEvent in loadedEvents)
            {
                var loadedEventString = Encoding.UTF8.GetString(loadedEvent.Data);
                Type eventType = Type.GetType(loadedEvent.Type);
                var convertedEvent = JsonConvert.DeserializeObject(loadedEventString, eventType);
                inventoryDomainEvents.Add(convertedEvent as InventoryDomainEvent);
            }
            aggregate.LoadEvents((await @events.ToListAsync()).Select(
                e => JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(e.Data),
                    Type.GetType(e.Type))) as IEnumerable<InventoryDomainEvent>);
            return aggregate.Build();
        }

        public Task DeleteAsync(Guid nintendoUserId)
        {
            return Task.CompletedTask;
        }
    }
}