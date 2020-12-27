namespace Inventory.UnitTests.Domain.CommandHandlerTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using AutoFixture;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Commands;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;
    using Moq;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class NewGameCommandHandlerTests
    {
        private static readonly Guid NintendoUserId = Guid.NewGuid();

        private readonly ICommandHandler<NewGameCommand> commandHandler;

        private readonly Mock<IDispatcherEvent> dispatcher;

        private readonly Mock<IInventoryRepository> eventStoreRepository;

        private readonly Fixture fixture = new Fixture();

        public NewGameCommandHandlerTests()
        {
            this.eventStoreRepository = new Mock<IInventoryRepository>();

            this.dispatcher = new Mock<IDispatcherEvent>();

            this.commandHandler = new InventoryCommandHandler(
                new Mock<IItemRepository>().Object,
                this.eventStoreRepository.Object,
                this.dispatcher.Object);
        }

        [Fact]
        public async Task GivenANewGame_WhenCreateGame_ThenShouldSaveAnInitialVersion()
        {
            // Arrange && Act
            await this.commandHandler.Handle(new NewGameCommand(NintendoUserId));

            // Assert
            this.eventStoreRepository
                .Verify(o => o.SaveAsync(It.IsAny<IInventory>()));
        }

        [Fact]
        public async Task GivenANewGame_WhenCreateGame_ThenShouldDispatchInventoryCreated()
        {
            // Arrange && Act
            await this.commandHandler.Handle(new NewGameCommand(NintendoUserId));

            // Assert
            this.dispatcher
                .Verify(o => o.Send(It.IsAny<InventoryCreated>()));
        }

        [Fact]
        public async Task GivenANewGame_WhenCreateGame_ThenShouldDeletePreviousGameForThisUser()
        {
            // Arrange && Act
            await this.commandHandler.Handle(new NewGameCommand(NintendoUserId));

            // Assert
            this.eventStoreRepository
                .Verify(o => o.DeleteAsync(NintendoUserId), Times.Once);
        }
    }
}
