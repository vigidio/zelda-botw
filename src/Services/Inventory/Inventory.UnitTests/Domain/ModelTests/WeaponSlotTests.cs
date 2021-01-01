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
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.WeaponSlot.TotalSize.Should().Be(8);
        }

        [Fact]
        public void GivenAEmptyWeaponSlot_WhenAddWeapon_ThenShouldBeAdded()
        {
            // Arrange
            var weapon = this.fixture.Create<Weapon>();

            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

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

            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);

            // Act
            inventory.AddItem(weapon);

            // Assert
            inventory.GetUncommitted().Should().HaveCount(2);
            inventory.GetUncommitted().Last().Version.Should().Be(1);
            inventory.GetUncommitted().Last().Should().BeOfType<WeaponAdded>();
            var weaponAdded = inventory.GetUncommitted().Last() as WeaponAdded;
            weaponAdded!.NintendoUserId.Should().Be(inventory.NintendoUserId);
            weaponAdded!.MajorVersion.Should().Be(inventory.MajorVersion);
            weaponAdded!.ItemId.Should().Be(weapon.Id);
            weaponAdded.Should().BeEquivalentTo(weapon, cfg => cfg.Excluding(o => o.Id));
        }

        [Fact]
        public async Task GivenAFullWeaponSlot_WhenAddWeapon_ThenShouldThrowAFullSlotException()
        {
            // Arrange
            var initialWeapons = this.fixture.CreateMany<Weapon>(8);

            var fakeLoadedInventory = new AggregateFactory.InventoryBuilder(Guid.NewGuid())
                .WithManyWeapons(initialWeapons)
                .Build();

            this.inventoryRepositoryMock
                .Setup(o => o.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(fakeLoadedInventory);

            var weapon = this.fixture.Create<Weapon>();

            var inventory =
                await this.inventoryRepositoryMock.Object.GetByIdAsync(
                    this.fixture.Create<Guid>(),
                    this.fixture.Create<int>());

            // Assert
            inventory.WeaponSlot.SlotBag.Should().HaveCount(8);

            // Act && Assert
            Assert.Throws<FullSlotException>(() => inventory.WeaponSlot.Add(weapon));
        }
    }
}
