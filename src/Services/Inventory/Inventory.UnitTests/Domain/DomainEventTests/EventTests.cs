using Inventory.Domain.UseCases.NewGame;

namespace Inventory.UnitTests.Domain.DomainEventTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.UseCases.AddItem;
    using Inventory.Domain.UseCases.SaveGame;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class EventTests
    {
        private const int InitialMajorVersion = 0;

        private static readonly Guid NintendoUserId = Guid.NewGuid();

        [Fact]
        public void GivenGameSavedEvent_ThenAllPropertiesShouldBePassed()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            // Act
            var gameSaved = new GameSaved(inventoryIdentifier, InitialMajorVersion);

            // Assert
            gameSaved.InventoryIdentifier.Should().NotBeEmpty();
            gameSaved.InventoryIdentifier.Should().Be($"{NintendoUserId}-{InitialMajorVersion}");
            gameSaved.MajorVersion.Should().Be(InitialMajorVersion);
        }

        [Fact]
        public void GivenInventoryCreatedEvent_ThenIdShouldNotBeEmpty()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            // Act
            var inventoryCreated = new InventoryCreated(inventoryIdentifier);

            // Assert
            inventoryCreated.InventoryIdentifier.Should().NotBeEmpty();
        }

        [Fact]
        public void GivenWeaponAddedEvent_ThenAllPropertiesShouldBePassed()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var weapon = new Weapon(
                null,
                "Master Sword",
                "The Legendary sword that seals the darkness.",
                30,
                40,
                "Undefined",
                "All",
                "One-handed");

            // Act
            var inventoryItemAdded = new WeaponAdded(inventoryIdentifier, InitialMajorVersion, weapon.Id, weapon.Name,
                weapon.Description, weapon.Strength, weapon.Durability, weapon.Material, weapon.Archetype,
                weapon.Hands);

            // Assert
            inventoryItemAdded.Should().BeEquivalentTo(weapon, options =>
                options.Excluding(o => o.Id));
            inventoryItemAdded.InventoryIdentifier.Should().Be(inventoryIdentifier);
            inventoryItemAdded.ItemId.Should().Be(weapon.Id);
        }

        [Fact]
        public void GivenMaterialAddedEvent_ThenAllPropertiesShouldBePassed()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var material = new Material(
                null,
                "Apple",
                "Apple item",
                2,
                0,
                MaterialType.Hearty);

            // Act
            var inventoryMaterialAdded = new MaterialAdded(inventoryIdentifier, InitialMajorVersion, material.Id,
                material.Name, material.Description, material.HP, material.Time, material.Type.ToString());

            // Assert
            inventoryMaterialAdded.Should().BeEquivalentTo(material, options =>
                options
                    .Excluding(o => o.Id)
                    .Excluding(o => o.Type));
            inventoryMaterialAdded.InventoryIdentifier.Should().Be(inventoryIdentifier);
            inventoryMaterialAdded.ItemId.Should().Be(material.Id);
            inventoryMaterialAdded.Type.Should().Be(material.Type.ToString());
        }

        [Fact]
        public void GivenMaterialRemovedEvent_ThenAllPropertiesShouldBePassed()
        {
            // Arrange
            var inventoryIdentifier = $"{NintendoUserId}-{InitialMajorVersion}";

            var materialId = Guid.NewGuid();

            // Act
            var inventoryMaterialRemoved = new MaterialRemoved(inventoryIdentifier, InitialMajorVersion, materialId);

            // Arrange
            inventoryMaterialRemoved.InventoryIdentifier.Should().Be(inventoryIdentifier);
            inventoryMaterialRemoved.ItemId.Should().Be(materialId);
        }
    }
}
