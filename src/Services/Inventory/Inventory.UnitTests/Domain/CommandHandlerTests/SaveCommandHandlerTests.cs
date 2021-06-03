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
    using Inventory.Domain.UseCases.AddItem;
    using Inventory.Domain.UseCases.NewGame;
    using Inventory.Domain.UseCases.SaveGame;
    using Moq;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class SaveCommandHandlerTests
    {
        private const int InitialMajorVersion = 0;

        private static readonly Guid NintendoUserId = Guid.NewGuid();

        private readonly ICommandHandler<SaveGameCommand> commandHandler;

        private readonly Mock<IDispatcherEvent> dispatcher;

        private readonly Mock<IInventoryRepository> eventStoreRepository;

        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IItemRepository> itemRepository;

        public SaveCommandHandlerTests()
        {
            this.dispatcher = new Mock<IDispatcherEvent>();

            this.eventStoreRepository = new Mock<IInventoryRepository>();

            this.itemRepository = new Mock<IItemRepository>();

            this.commandHandler = new SaveGameCommandHandler(
                this.eventStoreRepository.Object);
        }

        [Fact]
        public async Task GivenSaveCommand_WhenReceiveCommand_ThenShouldGetInventory()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var saveCommand = new SaveGameCommand(NintendoUserId, InitialMajorVersion);

            var fakeInventory = InventoryFactory.Create(NintendoUserId);

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(fakeInventory);

            // Act
            var result = await this.commandHandler.Handle(saveCommand);

            // Assert
            this.eventStoreRepository.VerifyAll();
        }

        [Fact]
        public async Task GivenSaveCommand_WhenInvalidInventory_ThenShouldThrowInvalidInventoryException()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var saveCommand = new SaveGameCommand(NintendoUserId, InitialMajorVersion);

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync((IInventory)null);

            // Act && Assert
            await Assert.ThrowsAsync<InvalidInventoryException>(() => this.commandHandler.Handle(saveCommand));
        }

        [Fact]
        public async Task GivenSaveCommand_WhenReceiveCommand_ThenGameSavedEventShouldOccur()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var saveCommand = new SaveGameCommand(NintendoUserId, InitialMajorVersion);

            var fakeInventory = InventoryFactory.Create(NintendoUserId);

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(fakeInventory);

            // Act
            var result = await this.commandHandler.Handle(saveCommand);

            // Assert
            result.GetLastCommitted().Last().Should().BeOfType<GameSaved>();
        }

        [Fact]
        public async Task GivenSaveCommand_WhenSave_ThenShouldNotHaveUncommittedChanges()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var saveCommand = new SaveGameCommand(NintendoUserId, InitialMajorVersion);

            var itemId = Guid.NewGuid();
            
            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync(this.fixture.Create<Weapon>());

            var fakeInventory = InventoryFactory.Create(NintendoUserId)
                .AddItem(this.fixture.Create<Weapon>());

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(fakeInventory);

            // Assert
            fakeInventory.GetUncommitted().Should().HaveCount(2);

            // Act
            var result = await this.commandHandler.Handle(saveCommand);

            // Assert
            result.GetUncommitted().Should().HaveCount(0);

            this.eventStoreRepository
                .Verify(o => o.SaveAsync(It.IsAny<IInventory>()), Times.Exactly(1));
        }

        [Fact]
        public async Task GivenSaveCommand_WhenSave_ThenVersionOfCommittedChangesShouldBeOrdered()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var saveCommand = new SaveGameCommand(NintendoUserId, InitialMajorVersion);

            var itemId = Guid.NewGuid();
            var addItemCommand = new AddItemCommand(inventoryIdentifier, itemId, ItemType.Weapon);

            this.itemRepository
                .Setup(o => o.GetByIdAsync(itemId.ToString()))
                .ReturnsAsync(this.fixture.Create<Weapon>());

            var fakeInventory = InventoryFactory.Create(NintendoUserId)
                .AddItem(this.fixture.Create<Weapon>())
                .AddItem(this.fixture.Create<Weapon>());

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(fakeInventory);
            
            // Assert
            fakeInventory.GetUncommitted().Should().HaveCount(3);

            // Act
            var result = await this.commandHandler.Handle(saveCommand);

            // Assert
            result.GetLastCommitted().Should().HaveCount(4);

            result.GetLastCommitted().First().Should().BeOfType<InventoryCreated>();
            result.GetLastCommitted().First().Version.Should().Be(0);

            result.GetLastCommitted().Skip(1).First().Should().BeOfType<WeaponAdded>();
            result.GetLastCommitted().First().Version.Should().Be(0);

            result.GetLastCommitted().Last().Should().BeOfType<GameSaved>();
            result.GetLastCommitted().Last().Version.Should().Be(3);

            this.eventStoreRepository
                .Verify(o => o.SaveAsync(It.IsAny<IInventory>()), Times.Exactly(1));
        }

        [Fact]
        public async Task GivenSaveCommand_WhenSave_ThenMajorVersionShouldBeIncremented()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var saveCommand = new SaveGameCommand(NintendoUserId, InitialMajorVersion);

            var fakeInventory = InventoryFactory.Create(NintendoUserId);

            this.eventStoreRepository
                .Setup(o => o.GetByIdAsync(inventoryIdentifier))
                .ReturnsAsync(fakeInventory);

            // Assert
            fakeInventory.MajorVersion.Should().Be(InitialMajorVersion);

            // Act
            var result = await this.commandHandler.Handle(saveCommand);

            // Assert
            result.MajorVersion.Should().Be(InitialMajorVersion + 1);
        }
    }
}
