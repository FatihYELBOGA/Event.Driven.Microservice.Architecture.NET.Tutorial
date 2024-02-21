using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {

        public event EventHandler<string> OnEventRemoved;
        public bool IsEmpty { get; }
        public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        public bool HasSubscriptionsForEvent(string eventName);
        public Type GetEventTypeByName(string eventName);
        public void Clear();
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        public string GetEventKey<T>();

    }

}
