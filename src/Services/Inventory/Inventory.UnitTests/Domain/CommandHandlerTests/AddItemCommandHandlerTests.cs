namespace Inventory.UnitTests.Domain.CommandHandlerTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class AddItemCommandHandlerTests
    {
        private const int InitialMajorVersion = 0;

        private static readonly Guid NintendoUserId = Guid.NewGuid();

        private readonly ICommandHandler<AddItemCommand> commandHandler;

        private readonly Mock<IDispatcherEvent> dispatcher;

        private readonly Mock<IInventoryRepository> eventStoreRepository;

        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IItemRepository> itemRepository;

        public AddItemCommandHandlerTests()
        {
            this.dispatcher = new Mock<IDispatcherEvent>();

            this.eventStoreRepository = new Mock<IInventoryRepository>();

            this.itemRepository = new Mock<IItemRepository>();

            this.commandHandler = new InventoryCommandHandler(
                this.itemRepository.Object,
                this.eventStoreRepository.Object,
                this.dispatcher.Object);
        }

        [Fact]
        public async Task GivenAddItemCommand_WhenInvalidItem_ThenShouldThrowInvalidItemException()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var itemId = Guid.NewGuid();
            var addItemCommand = new AddItemCommand(inventoryIdentifier, itemId, ItemType.Weapon);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync((Weapon)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidItemException>(() => this.commandHandler.Handle(addItemCommand));

            // Assert
            this.itemRepository.VerifyAll();
        }

        [Fact]
        public async Task GivenAddItemCommand_WhenInvalidInventory_ThenShouldThrowInvalidInventoryException()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var itemId = Guid.NewGuid();
            var addItemCommand = new AddItemCommand(inventoryIdentifier, itemId, ItemType.Weapon);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync(this.fixture.Create<Weapon>());

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync((IInventory)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidInventoryException>(() => this.commandHandler.Handle(addItemCommand));

            // Assert
            this.eventStoreRepository.VerifyAll();
        }

        [Fact]
        public async Task GivenAddItemCommand_WhenValidItem_ThenShouldAddToInventory()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var itemId = Guid.NewGuid();
            var addItemCommand = new AddItemCommand(inventoryIdentifier, itemId, ItemType.Weapon);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync(this.fixture.Create<Weapon>());

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(AggregateFactory.StartNew(NintendoUserId));

            this.eventStoreRepository
                .Setup(o => o.SaveAsync(It.IsAny<IInventory>()))
                .Returns(Task.CompletedTask);

            var resultChangesAddedOneItem = await this.commandHandler.Handle(addItemCommand);

            // Assert
            resultChangesAddedOneItem.GetUncommitted().Should().HaveCount(2);

            // Act
            var resultChangesAddedTwoItems = await this.commandHandler.Handle(addItemCommand);

            // Assert
            resultChangesAddedTwoItems.GetUncommitted().Should().HaveCount(3);

            this.eventStoreRepository.VerifyAll();
            this.itemRepository.VerifyAll();
        }
    }
}
