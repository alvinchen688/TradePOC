using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;

namespace TradePOC.Domain.Aggregates
{
    /// <summary>
    /// 聚合根
    /// </summary>
    public abstract class AggregateRoot
    {
        private readonly List<INotification> _domainEvents = new();
        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
