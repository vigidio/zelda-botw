namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Linq;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Models.Entity.Slot;
    using Inventory.Domain.UseCases.AddItem;
    using Inventory.Domain.UseCases.NewGame;
    using Inventory.Domain.UseCases.RemoveItem;
    using Inventory.Domain.UseCases.SaveGame;

    public class InventoryAggregate : AggregateRoot, IInventory
    {
        internal InventoryAggregate()
        {
            this.RegisterEvents();
            this.WeaponSlot = new WeaponSlot(null);
            this.ShieldSlot = new ShieldSlot(null);
            this.MaterialSlot = new MaterialSlot(null);
        }

        internal InventoryAggregate(
            Guid nintendoUserId,
            int version,
            ISingleSlot<Weapon> weaponSlot = null,
            ISingleSlot<Shield> shieldSlot = null,
            IStackSlot<Material> materialSlot = null)
        {
            this.MajorVersion = version;
            this.InventoryIdentifier = $"{nintendoUserId}-{this.MajorVersion}";
            this.WeaponSlot = weaponSlot ?? new WeaponSlot(null);
            this.ShieldSlot = shieldSlot ?? new ShieldSlot(null);
            this.MaterialSlot = materialSlot ?? new MaterialSlot(null);
        }

        public ISingleSlot<Weapon> WeaponSlot { get; }

        public ISingleSlot<Shield> ShieldSlot { get; }

        public IStackSlot<Material> MaterialSlot { get; }

        public int TotalItems => this.MaterialSlot.SlotBag.Count +
                                 this.WeaponSlot.SlotBag.Count +
                                 this.ShieldSlot.SlotBag.Count;

        public IInventory AddItem(IItem item)
        {
            switch (item)
            {
                case Weapon weapon:
                    this.WeaponSlot.Add(weapon);
                    this.ApplyEvent(this.CreateWeaponAddedEvent(weapon));
                    break;
                case Shield shield:
                    this.ShieldSlot.Add(shield);
                    this.ApplyEvent(new ShieldAdded(this.InventoryIdentifier, this.MajorVersion));
                    break;
                case Material material:
                    this.MaterialSlot.Add(material);
                    this.ApplyEvent(this.CreateMaterialAddedEvent(material));
                    break;
            }

            return this;
        }

        public IInventory RemoveItem(IItem item)
        {
            switch (item)
            {
                case Material material:
                    this.MaterialSlot.Remove(material.Id);
                    this.ApplyEvent(new MaterialRemoved(
                        this.InventoryIdentifier, this.MajorVersion, material.Id));
                    break;
                case Weapon weapon:
                    this.WeaponSlot.Remove(weapon.Id);
                    this.ApplyEvent(new WeaponRemoved(
                        this.InventoryIdentifier, this.MajorVersion, weapon.Id));
                    break;
                case Shield shield:
                    this.ShieldSlot.Remove(shield.Id);
                    this.ApplyEvent(new ShieldRemoved(
                        this.InventoryIdentifier, this.MajorVersion, shield.Id));
                    break;
            }

            return this;
        }

        public IInventory Save()
        {
            this.ApplyEvent(new GameSaved(this.InventoryIdentifier, this.MajorVersion));

            return this;
        }

        private MaterialAdded CreateMaterialAddedEvent(Material material)
        {
            return new MaterialAdded(
                this.InventoryIdentifier,
                this.MajorVersion,
                material.Id,
                material.Name,
                material.Description,
                material.HP,
                material.Time,
                material.Type.ToString());
        }

        private WeaponAdded CreateWeaponAddedEvent(Weapon weapon)
        {
            return new WeaponAdded(
                this.InventoryIdentifier,
                this.MajorVersion,
                weapon.Id,
                weapon.Name,
                weapon.Description,
                weapon.Strength,
                weapon.Durability,
                weapon.Material,
                weapon.Archetype,
                weapon.Hands);
        }

        private void RegisterEvents()
        {
            this.Register<InventoryCreated>(this.When);
            this.Register<WeaponAdded>(this.When);
            this.Register<MaterialAdded>(this.When);
            this.Register<MaterialRemoved>(this.When);
            this.Register<GameSaved>(this.When);
        }

        private void When(InventoryCreated e)
        {
            this.InventoryIdentifier = e.InventoryIdentifier;
            this.MajorVersion = e.MajorVersion;
        }

        private void When(GameSaved e)
        {
            this.InventoryIdentifier = e.InventoryIdentifier;
            this.MajorVersion = e.MajorVersion;
        }

        private void When(WeaponAdded e)
        {
            this.WeaponSlot.Add(new Weapon(
                e.ItemId,
                e.Name,
                e.Description,
                e.Strength,
                e.Durability,
                e.Material,
                e.Archetype,
                e.Hands));
        }

        private void When(MaterialAdded e)
        {
            var material = new Material(
                e.ItemId,
                e.Name,
                e.Description,
                e.HP,
                e.Time,
                (MaterialType)Enum.Parse(typeof(MaterialType), e.Type));

            this.MaterialSlot.Add(material);
        }

        private void When(MaterialRemoved e)
        {
            this.MaterialSlot.Remove(e.ItemId);
        }
    }
}
