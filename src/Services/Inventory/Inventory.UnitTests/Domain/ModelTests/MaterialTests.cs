namespace Inventory.UnitTests.Domain.ModelTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Inventory.Domain.Models.Entity;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class MaterialTests
    {
        [Fact]
        public void GivenAMaterial_WhenANewMaterialIsCreated_ThenShouldHaveAllParamsInformed()
        {
            // Arrange
            const string name = "Chillshroom";
            const string description = "Chillshroom item";
            const int hp = 2;
            const int time = 150;
            const MaterialType type = MaterialType.Chilly;

            // Act
            var material = new Material(null, name, description, hp, time, type);

            // Assert
            material.Id.Should().NotBe(Guid.Empty);
            material.Name.Should().Be(name);
            material.Description.Should().Be(description);
            material.HP.Should().Be(hp);
            material.Time.Should().Be(time);
            material.Type.Should().Be(type);
        }
    }
}
