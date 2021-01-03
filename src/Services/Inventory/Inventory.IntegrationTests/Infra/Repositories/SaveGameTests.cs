namespace Inventory.IntegrationTests.Infra.Repositories
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Commands;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Moq;
    using Xunit;

    [Collection("Sequential")]
    public class SaveGameTests
    {
        private static readonly Guid NintendoUserId = Guid.NewGuid();

        private readonly Fixture fixture = new Fixture();
        
        private readonly InventoryCommandHandler commandHandler;

        private readonly IInventoryRepository repository;

        private readonly Mock<IItemRepository> itemRepositoryMock;

        public SaveGameTests()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("IntegrationTest")
                .UseStartup<TestStartup>();

            var testServer = new TestServer(builder);

            this.itemRepositoryMock = new Mock<IItemRepository>();
            this.repository = testServer.Services.GetService(typeof(IInventoryRepository)) as InventoryRepository;

            this.commandHandler = new InventoryCommandHandler(
                this.itemRepositoryMock.Object, 
                this.repository, 
                new Mock<IDispatcherEvent>().Object);
        }

        //[Fact]
        public async Task GivenSomeChanges_WhenSaveGame_ShouldSaveOnDb()
        {
            // Arrange
            var newGameCommand = new NewGameCommand(NintendoUserId);
            
            await this.commandHandler.Handle(newGameCommand);
            
            var newGameSaved = await this.repository.GetByIdAsync(newGameCommand.NintendoUserId, 0);

            this.itemRepositoryMock
                .Setup(o => o.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(this.fixture.Create<Material>());

            await this.commandHandler.Handle(new AddItemCommand(
                newGameSaved.NintendoUserId,
                0,
                Guid.NewGuid(), 
                ItemType.Material));

            // Assert
            newGameSaved.MajorVersion.Should().Be(0);

            // Act
            await this.commandHandler.Handle(new SaveCommand(NintendoUserId, newGameSaved.MajorVersion));

            // Assert
            (await this.repository.GetByIdAsync(NintendoUserId, 1)).MajorVersion.Should().Be(1);
        }
    }
}