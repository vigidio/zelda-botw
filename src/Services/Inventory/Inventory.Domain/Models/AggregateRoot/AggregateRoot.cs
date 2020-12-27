namespace Inventory.Domain.Models.AggregateRoot
{
    using System;
    using System.Collections.Generic;
    using Inventory.Domain.DomainEvents;

    public abstract class AggregateRoot : IAggregateRoot, IAggregateChanges
    {
        public string InventoryIdentifier { get; set; }

        public int MajorVersion { get; protected set; } = 0;

        public int EventVersion { get; private set; } = -1;

        private readonly Dictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();

        private readonly List<Event> changes = new List<Event>();

        private List<Event> recentChanges = new List<Event>();

        public IEnumerable<Event> GetUncommitted() => this.changes;

        public IEnumerable<Event> GetLastCommitted() => this.recentChanges;

        public virtual void MarkChangesAsCommitted()
        {
            this.MajorVersion += 1;

            this.recentChanges = new List<Event>(this.changes);

            this.changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) this.ApplyEvent(e, false);
        }

        public void ApplyEvent(Event @event)
        {
            this.ApplyEvent(@event, true);
        }

        public void ApplyEvent(Event @event, bool isNew)
        {
            @event.Version = this.EventVersion += 1;
            if (isNew)
                this.changes.Add(@event);
            else
                this.Raise(@event);
        }

        protected void Register<T>(Action<T> when)
        {
            handlers.Add(typeof(T), e => when((T)e));
        }

        protected void Raise(object e)
        {
            handlers[e.GetType()](e);
        }
    }
}
