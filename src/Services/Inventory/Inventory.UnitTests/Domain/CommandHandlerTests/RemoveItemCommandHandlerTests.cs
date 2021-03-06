namespace Inventory.UnitTests.Domain.CommandHandlerTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Commands;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Exceptions;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Repositories;
    using Inventory.Domain.UseCases.RemoveItem;
    using Moq;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class RemoveItemCommandHandlerTests
    {
        private const int InitialMajorVersion = 0;

        private static readonly Guid NintendoUserId = Guid.NewGuid();

        private readonly ICommandHandler<RemoveItemCommand> commandHandler;

        private readonly Mock<IDispatcherEvent> dispatcher;

        private readonly Mock<IInventoryRepository> eventStoreRepository;

        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IItemRepository> itemRepository;

        public RemoveItemCommandHandlerTests()
        {
            this.dispatcher = new Mock<IDispatcherEvent>();

            this.eventStoreRepository = new Mock<IInventoryRepository>();

            this.itemRepository = new Mock<IItemRepository>();

            this.commandHandler = new RemoveItemCommandHandler(
                this.itemRepository.Object,
                this.eventStoreRepository.Object,
                this.dispatcher.Object);
        }

        [Fact]
        public async Task GivenRemoveItemCommand_WhenInvalidItem_ThenShouldThrowInvalidItemException()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var itemId = Guid.NewGuid();
            var itemType = ItemType.Material;
            var removeItemCommand = new RemoveItemCommand(inventoryIdentifier, itemId, itemType);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync((Material)null);

            // Act
            await Assert.ThrowsAsync<InvalidItemException>(() => this.commandHandler.Handle(removeItemCommand));

            // Assert
            this.itemRepository.VerifyAll();
        }

        [Fact]
        public async Task GivenRemoveItemCommand_WhenInvalidInventory_ThenShouldThrowInvalidInventoryException()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var itemId = Guid.NewGuid();
            var itemType = ItemType.Material;
            var removeItemCommand = new RemoveItemCommand(inventoryIdentifier, itemId, itemType);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync(this.fixture.Create<Material>());

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync((IInventory)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInventoryException>(() => this.commandHandler.Handle(removeItemCommand));

            // Assert
            this.eventStoreRepository.VerifyAll();
        }

        [Fact]
        public async Task GivenRemoveItemCommand_WhenValidItem_ThenShouldRemoveFromInventory()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var initialMaterials = this.fixture.CreateMany<Material>(160).ToList();

            var itemId = initialMaterials.First().Id;
            var itemType = ItemType.Material;
            var removeItemCommand = new RemoveItemCommand(inventoryIdentifier, itemId, itemType);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync(initialMaterials.First());

            var fakeLoadedInventory = InventoryFactory.Create(Guid.NewGuid());
            const int createdEventCount = 1;
            foreach (var initialMaterial in initialMaterials)
            {
                fakeLoadedInventory.AddItem(initialMaterial);
            }

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(fakeLoadedInventory);

            this.eventStoreRepository
                .Setup(o => o.SaveAsync(It.IsAny<IInventory>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultChangesRemovedTwoItems = await this.commandHandler.Handle(removeItemCommand);

            // Assert
            resultChangesRemovedTwoItems
                .GetUncommitted()
                .Should()
                .HaveCount(createdEventCount + initialMaterials.Count + 1);

            this.eventStoreRepository.VerifyAll();
            this.itemRepository.VerifyAll();
        }
    }
}
