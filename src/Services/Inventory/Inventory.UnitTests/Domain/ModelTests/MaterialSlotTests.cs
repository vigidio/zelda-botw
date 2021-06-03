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
    public class MaterialSlotTests
    {
        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IInventoryRepository> inventoryRepositoryMock;

        public MaterialSlotTests()
        {
            this.inventoryRepositoryMock = new Mock<IInventoryRepository>();
        }

        [Fact]
        public void GivenAMaterialSlot_WhenANewMaterialSlotIsCreated_TotalSizeOfSlotsShouldBeOneHundredAndSixty()
        {
            // Arrange && Act
            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.MaterialSlot.TotalSize.Should().Be(160);
        }

        [Fact]
        public void GivenAMaterialSlot_WhenANewMaterialSlotIsCreated_TotalStackSizeShouldBeNineHundredAndNinetyNine()
        {
            // Arrange && Act
            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.MaterialSlot.TotalQuantities.Should().Be(999);
        }

        [Fact]
        public void GivenAEmptyMaterialSlot_WhenAddMaterial_ThenShouldBeAdded()
        {
            // Arrange
            var material = new Material(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Acorn",
                "Acorn item",
                1,
                0,
                MaterialType.Hearty);

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.MaterialSlot.SlotBag.Should().HaveCount(0);

            // Act
            inventory.AddItem(material);

            // Assert
            inventory.MaterialSlot.SlotBag.Should().HaveCount(1);
        }

        [Fact]
        public void GivenAEmptyMaterialSlot_WhenAddTwoEqualMaterials_ThenOnlyOneSlotBagShouldBeOccupied()
        {
            // Arrange
            var material = new Material(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Acorn",
                "Acorn item",
                1,
                0,
                MaterialType.Hearty);

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.MaterialSlot.SlotBag.Should().HaveCount(0);

            // Act
            inventory.AddItem(material);
            inventory.AddItem(material);

            // Assert
            inventory.MaterialSlot.SlotBag.Should().HaveCount(1);
        }

        [Fact]
        public void GivenAMaterialSlotWith999SameItems_WhenAddOneMoreEqualItem_ThenShouldThrowAFullSlotException()
        {
            // Arrange
            var material = new Material(
                new Guid("00000000-0000-0000-0000-000000000001"),
                "Acorn",
                "Acorn Item",
                1,
                0,
                MaterialType.Hearty);

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            for (var i = 0; i < 999; i++)
            {
                inventory.MaterialSlot.Add(material);
            }

            // Assert
            inventory.MaterialSlot.SlotBag.First().Value.Should().HaveCount(999);

            // Act && Assert
            Assert.Throws<FullSlotException>(() => inventory.MaterialSlot.Add(material));
        }

        [Fact]
        public void GivenAEmptyMaterialSlot_WhenAddMaterial_ThenAnInventoryItemAddedEventShouldOccur()
        {
            // Arrange
            var material = new Material(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Acorn",
                "Acorn item",
                1,
                0,
                MaterialType.Hearty);

            var inventory = InventoryFactory.Create(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);

            // Act
            inventory.AddItem(material);

            // Assert
            inventory.GetUncommitted().Should().HaveCount(2);
            inventory.GetUncommitted().Last().Version.Should().Be(1);
            inventory.GetUncommitted().Last().Should().BeOfType<MaterialAdded>();
            var materialAdded = inventory.GetUncommitted().Last() as MaterialAdded;
            materialAdded!.ItemId.Should().Be(material.Id);
            materialAdded!.Type.Should().Be(material.Type.ToString());
            materialAdded.Should().BeEquivalentTo(material, cfg => cfg.Excluding(o => o.Id).Excluding(o => o.Type));
        }

        [Fact]
        public async Task GivenAFullMaterialSlot_WhenAddMaterial_ThenShouldThrowAFullSlotException()
        {
            // Arrange
            var initialMaterials = this.fixture.CreateMany<Material>(160);

            var fakeLoadedInventory = InventoryFactory.Create(Guid.NewGuid());
            foreach (var initialMaterial in initialMaterials)
            {
                fakeLoadedInventory.AddItem(initialMaterial);
            }

            this.inventoryRepositoryMock
                .Setup(o => o.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(fakeLoadedInventory);

            var material = this.fixture.Create<Material>();

            var inventory = await this.inventoryRepositoryMock.Object.GetByIdAsync(this.fixture.Create<string>());

            // Assert
            inventory.MaterialSlot.SlotBag.Should().HaveCount(160);

            // Act && Assert
            Assert.Throws<FullSlotException>(() => inventory.MaterialSlot.Add(material));
        }

        [Fact]
        public void GivenAMaterialSlotWithSomeMaterials_WhenRemovedAValidItemWithOnly1Quantity_ThenMaterialShouldBeRemoved()
        {
            // Arrange
            const int initialMaterialsCount = 160;
            var initialMaterials = this.fixture.CreateMany<Material>(initialMaterialsCount).ToList();

            var materialToRemove = initialMaterials.First();

            var inventory = InventoryFactory.Create(Guid.NewGuid());
            foreach (var initialMaterial in initialMaterials)
            {
                inventory.AddItem(initialMaterial);
            }
            
            // Assert
            inventory.MaterialSlot.SlotBag[materialToRemove.Id].Should().HaveCount(1);

            // Act
            inventory.RemoveItem(materialToRemove);

            // Assert
            inventory.MaterialSlot.SlotBag.Count.Should().Be(initialMaterialsCount - 1);
            inventory.MaterialSlot.SlotBag.ContainsKey(materialToRemove.Id).Should().BeFalse();
        }

        [Fact]
        public void GivenThreeItemsInTheInventory_WhenRemoveOneMaterial_ShouldHaveTwoItems()
        {
            // Arrange
            var material = fixture.Create<Material>();
            
            // Act
            var inventory = InventoryFactory.Create(Guid.NewGuid())
                .AddItem(material)
                .AddItem(fixture.Create<Shield>())
                .RemoveItem(material)
                .AddItem(fixture.Create<Weapon>());
            
            // Assert
            inventory.TotalItems.Should().Be(2);
        }

        [Fact]
        public void GivenAMaterialSlotWithSomeMaterials_WhenRemovedAValidItemWithMoreThan1Quantity_ThenQuantityForThisMaterialShouldBeDecremented()
        {
            // Arrange
            const int initialMaterialsCount = 160;
            var initialMaterials = this.fixture.CreateMany<Material>(initialMaterialsCount).ToList();

            initialMaterials.Add(initialMaterials.First());

            var materialToRemove = initialMaterials.First();

            var inventory = InventoryFactory.Create(Guid.NewGuid());
            foreach (var initialMaterial in initialMaterials)
            {
                inventory.AddItem(initialMaterial);
            }

            // Assert
            inventory.MaterialSlot.SlotBag[materialToRemove.Id].Should().HaveCount(2);

            // Act
            inventory.RemoveItem(materialToRemove);

            // Assert
            inventory.MaterialSlot.SlotBag.Count.Should().Be(initialMaterialsCount);
            inventory.MaterialSlot.SlotBag.ContainsKey(materialToRemove.Id).Should().BeTrue();
            inventory.MaterialSlot.SlotBag[materialToRemove.Id].Should().HaveCount(1);
        }

        [Fact]
        public void GivenAMaterialSlotWithSomeMaterials_WhenRemovedAValidItem_ThenMaterialRemovedEventShouldOccur()
        {
            // Arrange
            var initialMaterials = this.fixture.CreateMany<Material>(160).ToList();

            var materialToRemove = initialMaterials.First();

            var inventory = InventoryFactory.Create(Guid.NewGuid());
            foreach (var initialMaterial in initialMaterials)
            {
                inventory.AddItem(initialMaterial);
            }

            // Assert
            inventory.GetUncommitted().Should().HaveCount(initialMaterials.Count + 1);

            // Act
            inventory.RemoveItem(materialToRemove);

            // Assert
            inventory.GetUncommitted().Should().HaveCount(initialMaterials.Count + 2);
            inventory.GetUncommitted().Last().Version.Should().Be(initialMaterials.Count + 1);
            inventory.GetUncommitted().Last().Should().BeOfType<MaterialRemoved>();
            var materialRemoved = inventory.GetUncommitted().Last() as MaterialRemoved;
            materialRemoved!.InventoryIdentifier.Should().Be(inventory.InventoryIdentifier);
            materialRemoved!.MajorVersion.Should().Be(inventory.MajorVersion);
            materialRemoved!.ItemId.Should().Be(materialToRemove.Id);
        }
    }
}
