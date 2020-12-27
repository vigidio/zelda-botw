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

            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

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

            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

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

            var fakeLoadedInventory = new AggregateFactory.InventoryBuilder(Guid.NewGuid())
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
    }
}
