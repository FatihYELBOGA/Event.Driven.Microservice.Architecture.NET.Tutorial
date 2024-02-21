using EventBus.Base;
using EventBus.RabbitMQ;
using EventBus.Base.Abstraction;
using EventBus.AzureServiceBus;

namespace EventBus.Factory
{
    public class EventBusFactory
    {

        public static IEventBus Create(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
        {
            return eventBusConfig.EventBusType switch {
                EventBusType.AzureServiceBus => new EventBusServiceBus(eventBusConfig, serviceProvider),
                _ => new EventBusRabbitMQ(eventBusConfig, serviceProvider),
            };
        }

    }

}
