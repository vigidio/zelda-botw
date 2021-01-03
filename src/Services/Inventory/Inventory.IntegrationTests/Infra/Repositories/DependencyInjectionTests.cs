namespace Inventory.IntegrationTests.Infra.Repositories
{
    using FluentAssertions;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class InventoryRepositoryDependencyInjectionTests
    {
        private TestServer testServer;

        public InventoryRepositoryDependencyInjectionTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<TestStartup>();

            this.testServer = new TestServer(builder);
        }
        
        [Fact]
        public void GivenIInventoryRepository_WhenGetService_ThenInventoryRepositoryInstanceShouldBeCreated()
        {
            this.testServer.Services.GetService(typeof(IInventoryRepository)).Should().BeOfType<InventoryRepository>();
        }
    }
}