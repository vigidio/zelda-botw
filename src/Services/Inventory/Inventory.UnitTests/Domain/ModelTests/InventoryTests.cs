namespace Inventory.UnitTests.Domain.ModelTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.AggregateRoot;
    using Xunit;
    
    [ExcludeFromCodeCoverage]
    public class InventoryTests
    {
        [Fact]
        public void GivenANewGame_WhenCreateInventory_ThenSuccess()
        {
            // Arrange & Act
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.Should().NotBeNull();
            inventory.InventoryIdentifier.Should().NotBeNullOrEmpty();
            inventory.MajorVersion.Should().Be(0);
        }

        [Fact]
        public void GivenANewGame_WhenCreateInventory_ThenAnInventoryCreatedEventShouldOccur()
        {
            // Arrange & Act
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);
            inventory.GetUncommitted().Last().Should().BeOfType<InventoryCreated>();
            inventory.GetUncommitted().Last().Version.Should().Be(0);
        }

        [Fact]
        public void GivenANewGame_WhenInventoryCreated_ThenShouldHave8SlotsForWeapon()
        {
            // Arrange & Act
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.WeaponSlot.TotalSize.Should().Be(8);
        }

        [Fact]
        public void GivenANewGame_WhenInventoryCreated_ThenShouldHave4SlotsForShields()
        {
            // Arrange & Act
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.ShieldSlot.TotalSize.Should().Be(4);
        }

        [Fact]
        public void GivenAListOfChanges_WhenMarkChangesAsCommitted_ThenRecentChangesShouldStoreCommittedChanges()
        {
            // Arrange
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.GetLastCommitted().Should().HaveCount(0);

            // Act
            inventory.MarkChangesAsCommitted();

            // Assert
            inventory.GetLastCommitted().Should().HaveCount(1);
        }

        [Fact]
        public void GivenAListOfChanges_WhenMarkChangesAsCommitted_ThenChangesShouldBeClear()
        {
            // Arrange
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);

            // Act
            inventory.MarkChangesAsCommitted();

            // Assert
            inventory.GetUncommitted().Should().HaveCount(0);
        }

        [Fact]
        public void GivenAListOfChanges_WhenMarkChangesAsCommitted_ThenAMajorVersionShouldBeIncremented()
        {
            // Arrange
            var inventory = AggregateFactory.StartNew(Guid.NewGuid());

            // Assert
            inventory.GetUncommitted().Should().HaveCount(1);
            var initialMajorVersion = inventory.MajorVersion;

            // Act
            inventory.MarkChangesAsCommitted();

            // Assert
            inventory.MajorVersion.Should().Be(initialMajorVersion + 1);
        }

        [Theory]
        [ClassData(typeof(InventoryProjectionDataTests))]
        public void GivenAListOfEvents_WhenLoadFromHistory_ThenShouldBuildTheAggregate(
            List<Event> @events,
            IInventory expectedInventory)
        {
            // Arrange
            var nintendoUserId = Guid.Parse("c1ffe919-facf-4e1d-a765-477a1d13ee2e");

            var inventoryBuilder = new AggregateFactory.HistoryBuilder(nintendoUserId)
                .LoadEvents(events);

            // Act
            var aggregate = inventoryBuilder.Build();

            // Assert
            aggregate.EventVersion.Should().Be(events.Count - 1);
            aggregate.InventoryIdentifier.Should().Be(expectedInventory.InventoryIdentifier);
            aggregate.MajorVersion.Should().Be(expectedInventory.MajorVersion);
            aggregate.WeaponSlot.Should().BeEquivalentTo(expectedInventory.WeaponSlot);
            aggregate.ShieldSlot.Should().BeEquivalentTo(expectedInventory.ShieldSlot);
            aggregate.MaterialSlot.Should().BeEquivalentTo(expectedInventory.MaterialSlot);
        }
    }
}
