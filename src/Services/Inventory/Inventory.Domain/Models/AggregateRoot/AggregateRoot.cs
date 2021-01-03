namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;

    public abstract class AggregateRoot : IAggregateChanges
    {
        public Guid NintendoUserId { get; protected set; }

        public int MajorVersion { get; protected set; } = 0;

        public int EventVersion { get; private set; } = -1;

        private readonly Dictionary<Type, Action<object>> handlers = new();

        private readonly List<InventoryDomainEvent> changes = new();

        private List<InventoryDomainEvent> recentChanges = new();

        public IEnumerable<InventoryDomainEvent> GetUncommitted() => this.changes;

        public IEnumerable<InventoryDomainEvent> GetLastCommitted() => this.recentChanges;

        public virtual void MarkChangesAsCommitted()
        {
            this.recentChanges = new List<InventoryDomainEvent>(this.changes);

            this.changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<InventoryDomainEvent> history)
        {
            foreach (var e in history) this.ApplyEvent(e, false);
        }

        public void ApplyEvent(InventoryDomainEvent @event)
        {
            this.ApplyEvent(@event, true);
        }

        private void ApplyEvent(InventoryDomainEvent @event, bool isNew)
        {
            if (isNew)
                this.changes.Add(@event);
            else
                this.Raise(@event);
        }

        protected void Register<T>(Action<T> when)
        {
            this.handlers.Add(typeof(T), e => when((T)e));
        }

        private void Raise(object e)
        {
            this.handlers[e.GetType()](e);
        }
    }
}
