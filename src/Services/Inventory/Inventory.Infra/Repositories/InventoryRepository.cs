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
    using Newtonsoft.Json.Serialization;

    /// <summary>
    ///     InventoryRepository class.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly IMongoCollection<IEventData> inventoryCollection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InventoryRepository" /> class.
        /// </summary>
        public InventoryRepository(IMongoCollection<IEventData> inventoryCollection)
        {
            this.inventoryCollection = inventoryCollection;
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
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, DefaultSettings)),
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
            var inventoryDomainEvents = loadedEvents.Select(loadedEvent => 
                JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(loadedEvent.Data), 
                    Type.GetType($"Inventory.Domain.DomainEvents.{loadedEvent.Type}, Inventory.Domain", true), 
                    DefaultSettings)).
                ToList();

            return aggregate.Build();
        }

        public Task DeleteAsync(Guid nintendoUserId)
        {
            return Task.CompletedTask;
        }
    }
}