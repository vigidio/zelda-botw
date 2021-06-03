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
    using Moq;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class WeaponSlotTests
    {
        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IInventoryRepository> inventoryRepositoryMock;

        public WeaponSlotTests()
        {
            this.inventoryRepositoryMock = new Mock<IInventoryRepository>();
        }

        [Fact]
        public void GivenAWeaponSlot_WhenANewWeaponSlotIsCreated_TotalSizeOfSlotsShouldBeEight()
        {
            // Arrange && Act
            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.WeaponSlot.TotalSize.Should().Be(8);
        }

        [Fact]
        public void GivenAEmptyWeaponSlot_WhenAddWeapon_ThenShouldBeAdded()
        {
            // Arrange
            var weapon = this.fixture.Create<Weapon>();

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.WeaponSlot.SlotBag.Should().HaveCount(0);

            // Act
            inventory.AddItem(weapon);

            // Assert
            inventory.WeaponSlot.SlotBag.Should().HaveCount(1);
        }

        [Fact]
        public void GivenAEmptyWeaponSlot_WhenAddWeapon_ThenAnInventoryItemAddedEventShouldOccur()
        {
            // Arrange
            var weapon = this.fixture.Create<Weapon>();

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);

            // Act
            inventory.AddItem(weapon);

            // Assert
            inventory.GetUncommitted().Should().HaveCount(2);
            inventory.GetUncommitted().Last().Version.Should().Be(1);
            inventory.GetUncommitted().Last().Should().BeOfType<WeaponAdded>();
            var weaponAdded = inventory.GetUncommitted().Last() as WeaponAdded;
            weaponAdded!.InventoryIdentifier.Should().Be(inventory.InventoryIdentifier);
            weaponAdded!.MajorVersion.Should().Be(inventory.MajorVersion);
            weaponAdded!.ItemId.Should().Be(weapon.Id);
            weaponAdded.Should().BeEquivalentTo(weapon, cfg => cfg.Excluding(o => o.Id));
        }

        [Fact]
        public async Task GivenAFullWeaponSlot_WhenAddWeapon_ThenShouldThrowAFullSlotException()
        {
            // Arrange
            var initialWeapons = this.fixture.CreateMany<Weapon>(8);

            var fakeLoadedInventory = InventoryFactory.Create(Guid.NewGuid());
            foreach (var initialWeapon in initialWeapons)
            {
                fakeLoadedInventory.AddItem(initialWeapon);
            }

            this.inventoryRepositoryMock
                .Setup(o => o.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(fakeLoadedInventory);

            var weapon = this.fixture.Create<Weapon>();

            var inventory = await this.inventoryRepositoryMock.Object.GetByIdAsync(this.fixture.Create<string>());

            // Assert
            inventory.WeaponSlot.SlotBag.Should().HaveCount(8);

            // Act && Assert
            Assert.Throws<FullSlotException>(() => inventory.WeaponSlot.Add(weapon));
        }
        
        [Fact]
        public void GivenThreeItemsInTheInventory_WhenRemoveOneWeapon_ShouldHaveTwoItems()
        {
            // Arrange
            var weapon = fixture.Create<Weapon>();
            
            // Act
            var inventory = InventoryFactory.Create(Guid.NewGuid())
                .AddItem(weapon)
                .AddItem(fixture.Create<Shield>())
                .RemoveItem(weapon)
                .AddItem(fixture.Create<Weapon>());
            
            // Assert
            inventory.TotalItems.Should().Be(2);
        }
        
        [Fact]
        public void GivenAWeaponSlotWithSomeWeapons_WhenRemovedAValidItem_ThenWeaponRemovedEventShouldOccur()
        {
            // Arrange
            var initialWeapons = this.fixture.CreateMany<Weapon>(6).ToList();

            var weaponToRemove = initialWeapons.First();

            var inventory = InventoryFactory.Create(Guid.NewGuid());
            foreach (var initialWeapon in initialWeapons)
            {
                inventory.AddItem(initialWeapon);
            }

            // Assert
            inventory.GetUncommitted().Should().HaveCount(initialWeapons.Count + 1);

            // Act
            inventory.RemoveItem(weaponToRemove);

            // Assert
            inventory.GetUncommitted().Should().HaveCount(initialWeapons.Count + 2);
            inventory.GetUncommitted().Last().Version.Should().Be(initialWeapons.Count + 1);
            inventory.GetUncommitted().Last().Should().BeOfType<WeaponRemoved>();
            var weaponRemoved = inventory.GetUncommitted().Last() as WeaponRemoved;
            weaponRemoved!.InventoryIdentifier.Should().Be(inventory.InventoryIdentifier);
            weaponRemoved!.MajorVersion.Should().Be(inventory.MajorVersion);
            weaponRemoved!.ItemId.Should().Be(weaponToRemove.Id);
        }
    }
}
