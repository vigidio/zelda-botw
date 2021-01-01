namespace Inventory.IntegrationTests.Infra.Repositories
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.Commands;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Moq;
    using Xunit;

    [Collection("Sequential")]
    public class NewGameTests
    {
        private static readonly Guid NintendoUserId = Guid.NewGuid();

        private readonly InventoryCommandHandler commandHandler;

        private readonly IInventoryRepository repository;

        public NewGameTests()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("IntegrationTest")
                .UseStartup<TestStartup>();

            var testServer = new TestServer(builder);

            this.repository = testServer.Services.GetService(typeof(IInventoryRepository)) as InventoryRepository;

            this.commandHandler = new InventoryCommandHandler(
                new Mock<IItemRepository>().Object, 
                this.repository, 
                new Mock<IDispatcherEvent>().Object);
        }

        [Fact]
        public async Task GivenANewGame_WhenCreateGame_ShouldSaveOnDb()
        {
            // Arrange
            var newGameCommand = new NewGameCommand(NintendoUserId);

            // Act
            await this.commandHandler.Handle(newGameCommand);

            // Assert
            (await this.repository.GetByIdAsync(newGameCommand.NintendoUserId, 0)).Should().NotBeNull();
        }
    }
}
