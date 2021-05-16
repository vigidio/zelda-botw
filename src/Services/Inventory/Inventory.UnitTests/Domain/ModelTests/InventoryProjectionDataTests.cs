using Inventory.Domain.UseCases.NewGame;

namespace Inventory.UnitTests.Domain.ModelTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.UseCases.AddItem;
    using Inventory.Domain.UseCases.SaveGame;

    [ExcludeFromCodeCoverage]
    public class InventoryProjectionDataTests : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                InventoryScenario.FirstInputScenario(), InventoryScenario.FirstExpectedAggregate(),
            };
            yield return new object[]
            {
                InventoryScenario.SecondInputScenario(), InventoryScenario.SecondExpectedAggregate(),
            };
            yield return new object[]
            {
                InventoryScenario.ThirdInputScenario(), InventoryScenario.ThirdExpectedAggregate(),
            };
            yield return new object[]
            {
                InventoryScenario.FourthInputScenario(), InventoryScenario.FourthExpectedAggregate(),
            };
            yield return new object[]
            {
                InventoryScenario.FifthInputScenario(), InventoryScenario.FifthExpectedAggregate(),
            };
            yield return new object[]
            {
                InventoryScenario.SixthInputScenario(), InventoryScenario.SixthExpectedAggregate(),
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    internal static class InventoryScenario
    {
        private const int InitialMajorVersion = 0;

        private static readonly Guid NintendoUserId = Guid.Parse("c1ffe919-facf-4e1d-a765-477a1d13ee2e");

        private static readonly Weapon Weapon = new Weapon(
            Guid.Parse("6f12c20a-b38d-439c-8f9b-55fab24c7007"),
            "Master Sword",
            "The Legendary sword that seals the darkness.",
            30,
            40,
            "Undefined",
            "All",
            "One-handed");

        private static readonly Shield Shield = new Shield();

        private static readonly Material Material = new Material(
            Guid.Parse("d31acc71-b434-4136-b0e1-98f7bf7f09a6"),
            "Apple",
            "Fruit that restore health",
            2,
            0,
            MaterialType.Nothing);

        public static List<Event> FirstInputScenario() =>
            new List<Event>
            {
                new InventoryCreated($"{NintendoUserId}-{InitialMajorVersion}"),
            };

        public static IInventory FirstExpectedAggregate() =>
            new InventoryFactory.InventoryBuilder(NintendoUserId)
                .Build();

        public static List<Event> SecondInputScenario() =>
            new List<Event>
            {
                new InventoryCreated($"{NintendoUserId}-{InitialMajorVersion}"),

                new WeaponAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Weapon.Id,
                    Weapon.Name,
                    Weapon.Description,
                    Weapon.Strength,
                    Weapon.Durability,
                    Weapon.Material,
                    Weapon.Archetype,
                    Weapon.Hands),
            };

        public static IInventory SecondExpectedAggregate() =>
            new InventoryFactory.InventoryBuilder(NintendoUserId)
                .WithManyWeapons(new List<Weapon> { Weapon })
                .Build();

        public static List<Event> ThirdInputScenario() =>
            new List<Event>
            {
                new InventoryCreated($"{NintendoUserId}-{InitialMajorVersion}"),

                new WeaponAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Weapon.Id,
                    Weapon.Name,
                    Weapon.Description,
                    Weapon.Strength,
                    Weapon.Durability,
                    Weapon.Material,
                    Weapon.Archetype,
                    Weapon.Hands),

                new GameSaved($"{NintendoUserId}-{InitialMajorVersion + 1}", InitialMajorVersion + 1),
            };

        public static IInventory ThirdExpectedAggregate() =>
            new InventoryFactory.InventoryBuilder(NintendoUserId, InitialMajorVersion + 1)
                .WithManyWeapons(new List<Weapon> { Weapon })
                .Build();

        public static List<Event> FourthInputScenario() =>
            new List<Event>
            {
                new InventoryCreated($"{NintendoUserId}-{InitialMajorVersion}"),

                new MaterialAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id,
                    Material.Name,
                    Material.Description,
                    Material.HP,
                    Material.Time,
                    Material.Type.ToString()),

                new MaterialAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id,
                    Material.Name,
                    Material.Description,
                    Material.HP,
                    Material.Time,
                    Material.Type.ToString()),
            };

        public static IInventory FourthExpectedAggregate() =>
            new InventoryFactory.InventoryBuilder(NintendoUserId)
                .WithManyMaterials(new List<Material> { Material, Material })
                .Build();

        public static List<Event> FifthInputScenario() =>
            new List<Event>
            {
                new InventoryCreated($"{NintendoUserId}-{InitialMajorVersion}"),

                new MaterialAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id,
                    Material.Name,
                    Material.Description,
                    Material.HP,
                    Material.Time,
                    Material.Type.ToString()),

                new MaterialAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id,
                    Material.Name,
                    Material.Description,
                    Material.HP,
                    Material.Time,
                    Material.Type.ToString()),

                new MaterialRemoved(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id),
            };

        public static IInventory FifthExpectedAggregate() =>
            new InventoryFactory.InventoryBuilder(NintendoUserId)
                .WithManyMaterials(new List<Material> { Material })
                .Build();

        public static List<Event> SixthInputScenario() =>
            new List<Event>
            {
                new InventoryCreated($"{NintendoUserId}-{InitialMajorVersion}"),

                new MaterialAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id,
                    Material.Name,
                    Material.Description,
                    Material.HP,
                    Material.Time,
                    Material.Type.ToString()),

                new MaterialAdded(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id,
                    Material.Name,
                    Material.Description,
                    Material.HP,
                    Material.Time,
                    Material.Type.ToString()),

                new MaterialRemoved(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id),

                new MaterialRemoved(
                    $"{NintendoUserId}-{InitialMajorVersion}",
                    InitialMajorVersion,
                    Material.Id),
            };

        public static IInventory SixthExpectedAggregate() =>
            new InventoryFactory.InventoryBuilder(NintendoUserId)
                .WithManyMaterials(new List<Material>())
                .Build();
    }
}
