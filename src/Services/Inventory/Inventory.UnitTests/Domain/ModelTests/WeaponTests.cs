namespace Inventory.UnitTests.Domain.ModelTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Inventory.Domain.Models.Entity;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class WeaponTests
    {
        [Fact]
        public void GivenAWeapon_WhenANewWeaponCreated_ThenShouldHaveAllParamsInformed()
        {
            // Arrange
            const string name = "Master Sword";
            const string description = "The Legendary sword that seals the darkness.";
            const int strength = 30;
            const int durability = 40;
            const string material = "Undefined";
            const string archetype = "All";
            const string hands = "One-handed";

            // Act
            var weapon = new Weapon(
                null,
                name,
                description,
                strength,
                durability,
                material,
                archetype,
                hands);

            // Assert
            weapon.Id.Should().NotBe(Guid.Empty);
            weapon.Name.Should().Be(name);
            weapon.Description.Should().Be(description);
            weapon.Strength.Should().Be(strength);
            weapon.Durability.Should().Be(durability);
            weapon.Material.Should().Be(material);
            weapon.Archetype.Should().Be(archetype);
            weapon.Hands.Should().Be(hands);
        }
    }
}
