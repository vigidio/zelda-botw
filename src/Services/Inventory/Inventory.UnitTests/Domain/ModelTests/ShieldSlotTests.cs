namespace Inventory.UnitTests.Domain.ModelTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Exceptions;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Repositories;
    using Inventory.Domain.UseCases.AddItem;
    using Inventory.Domain.UseCases.RemoveItem;
    using Moq;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class ShieldSlotTests
    {
        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IInventoryRepository> inventoryRepositoryMock;

        public ShieldSlotTests()
        {
            this.inventoryRepositoryMock = new Mock<IInventoryRepository>();
        }

        [Fact]
        public void GivenAEmptyShieldSlot_WhenAddShield_ThenShouldBeAdded()
        {
            // Arrange
            var shield = this.fixture.Create<Shield>();

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.ShieldSlot.SlotBag.Should().HaveCount(0);

            // Act
            inventory.AddItem(shield);

            // Assert
            inventory.ShieldSlot.SlotBag.Should().HaveCount(1);
        }

        [Fact]
        public void GivenAEmptyShieldSlot_WhenAddShield_ThenAnInventoryItemAddedEventShouldOccur()
        {
            // Arrange
            var shield = this.fixture.Create<Shield>();

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);

            // Act
            inventory.AddItem(shield);

            // Assert
            inventory.GetUncommitted().Should().HaveCount(2);
            inventory.GetUncommitted().Last().Should().BeOfType<ShieldAdded>();
        }

        [Fact]
        public async Task GivenAFullShieldSlot_WhenAddShield_ThenShouldThrowAFullSlotException()
        {
            // Arrange
            var initialShields = this.fixture.CreateMany<Shield>(4);

            var fakeLoadedInventory = new InventoryFactory.InventoryBuilder(Guid.NewGuid())
                .WithManyShields(initialShields)
                .Build();

            this.inventoryRepositoryMock
                .Setup(o => o.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(fakeLoadedInventory);

            var shield = this.fixture.Create<Shield>();

            var inventory = await this.inventoryRepositoryMock.Object.GetByIdAsync(this.fixture.Create<string>());

            // Assert
            inventory.ShieldSlot.SlotBag.Should().HaveCount(4);

            // Act && Assert
            Assert.Throws<FullSlotException>(() => inventory.ShieldSlot.Add(shield));
        }
        
        [Fact]
        public void GivenThreeItemsInTheInventory_WhenRemoveOneShield_ShouldHaveTwoItems()
        {
            // Arrange
            var shield = fixture.Create<Shield>();
            
            // Act
            var inventory = InventoryFactory.Create(Guid.NewGuid())
                .AddItem(shield)
                .AddItem(fixture.Create<Material>())
                .RemoveItem(shield)
                .AddItem(fixture.Create<Weapon>());
            
            // Assert
            inventory.TotalItems.Should().Be(2);
        }
        
        [Fact]
        public void GivenAShieldSlotWithSomeShields_WhenRemovedAValidItem_ThenShieldRemovedEventShouldOccur()
        {
             // Arrange
             var initialMaterials = this.fixture.CreateMany<Shield>(4);
        
             var shieldToRemove = initialMaterials.First();
        
             var inventory = new InventoryFactory.InventoryBuilder(Guid.NewGuid())
                 .WithManyShields(initialMaterials)
                 .Build();
        
             // Assert
             inventory.GetUncommitted().Should().HaveCount(0);
        
             // Act
             inventory.RemoveItem(shieldToRemove);
        
             // Assert
             inventory.GetUncommitted().Should().HaveCount(1);
             inventory.GetUncommitted().Last().Version.Should().Be(0);
             inventory.GetUncommitted().Last().Should().BeOfType<ShieldRemoved>();
             var shieldRemoved = inventory.GetUncommitted().Last() as ShieldRemoved;
             shieldRemoved!.InventoryIdentifier.Should().Be(inventory.InventoryIdentifier);
             shieldRemoved!.MajorVersion.Should().Be(inventory.MajorVersion);
             shieldRemoved!.ItemId.Should().Be(shieldToRemove.Id);
        }
    }
}
