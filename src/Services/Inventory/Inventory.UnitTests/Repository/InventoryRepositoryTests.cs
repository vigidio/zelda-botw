namespace Inventory.UnitTests.Repository
{
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Configurations;
    using Inventory.Infra.Repositories;
    using Moq;
    using Xunit;

    public class InventoryRepositoryTests
    {
        private readonly IInventoryRepository inventoryRepository;

        public InventoryRepositoryTests()
        {
            var repositoryConfiguration = new Mock<IRepositoryConfiguration>();
            this.inventoryRepository = new InventoryRepository(repositoryConfiguration.Object);
        }
        
        [Fact]
        public void GivenAnAggregate_WhenSave_ThenShouldCallSaveAsync()
        {
            // Arrange
    
            // Act
            this.inventoryRepository.SaveAsync(null);

            // Assert
        }   
    }
}