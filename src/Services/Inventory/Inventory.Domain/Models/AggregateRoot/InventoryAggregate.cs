namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Models.Entity;
    using Inventory.Domain.Models.Entity.Slot;

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
            this.NintendoUserId = nintendoUserId;
            this.MajorVersion = version;
            this.WeaponSlot = weaponSlot ?? new WeaponSlot(null);
            this.ShieldSlot = shieldSlot ?? new ShieldSlot(null);
            this.MaterialSlot = materialSlot ?? new MaterialSlot(null);
        }

        public ISingleSlot<Weapon> WeaponSlot { get; }

        public ISingleSlot<Shield> ShieldSlot { get; }

        public IStackSlot<Material> MaterialSlot { get; }

        public void AddItem(IItem item)
        {
            switch (item)
            {
                case Weapon weapon:
                    this.WeaponSlot.Add(weapon);
                    this.ApplyEvent(this.CreateWeaponAddedEvent(weapon));
                    break;
                case Shield shield:
                    this.ShieldSlot.Add(shield);
                    this.ApplyEvent(new ShieldAdded(this.NintendoUserId, this.MajorVersion));
                    break;
                case Material material:
                    this.MaterialSlot.Add(material);
                    this.ApplyEvent(this.CreateMaterialAddedEvent(material));
                    break;
            }
        }

        public void RemoveItem(IItem item)
        {
            switch (item)
            {
                case Material material:
                    this.MaterialSlot.Remove(material.Id);
                    this.ApplyEvent(new MaterialRemoved(
                        this.NintendoUserId, this.MajorVersion, material.Id));
                    break;
            }
        }

        public void Save()
        {
            this.ApplyEvent(new GameSaved(this.NintendoUserId, this.MajorVersion));
            this.MajorVersion += 1;
        }

        private MaterialAdded CreateMaterialAddedEvent(Material material)
        {
            return new MaterialAdded(
                this.NintendoUserId,
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
                this.NintendoUserId,
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
            this.NintendoUserId = e.NintendoUserId;
            this.MajorVersion = e.Version;
        }

        private void When(GameSaved e)
        {
            this.NintendoUserId = e.NintendoUserId;
            this.MajorVersion = e.Version;
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
