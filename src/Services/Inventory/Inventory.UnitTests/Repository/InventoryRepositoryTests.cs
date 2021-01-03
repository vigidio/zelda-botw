namespace Inventory.UnitTests.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AutoFixture;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Models;
    using Inventory.Infra.Repositories;
    using MongoDB.Driver;
    using Moq;
    using Xunit;

    public class InventoryRepositoryTests
    {
        private readonly Fixture fixture = new();

        private readonly IInventoryRepository inventoryRepository;

        private readonly Mock<IMongoCollection<IEventData>> inventoryCollection;

        public InventoryRepositoryTests()
        {
            this.inventoryCollection = new Mock<IMongoCollection<IEventData>>();
            this.inventoryRepository = new InventoryRepository(this.inventoryCollection.Object);
        }

        [Fact]
        public void GivenAnAggregateWithOneOrMoreUncommittedChanges_WhenSave_ThenShouldCallSaveAsync()
        {
            // Arrange
            var aggregate = AggregateFactory.StartNew(Guid.NewGuid());
            aggregate.AddItem(this.fixture.Create<Material>());

            // Act
            this.inventoryRepository.SaveAsync(aggregate);

            // Assert
            this.inventoryCollection.Verify(
                o => o.InsertManyAsync(
                    It.IsAny<IEnumerable<EventData>>(), It.IsAny<InsertManyOptions>(), CancellationToken.None));
        }
    }
}