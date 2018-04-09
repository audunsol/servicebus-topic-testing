using Microsoft.Extensions.Configuration;

namespace service_bus_testing
{
    public interface IConfig
    {
        IConfigurationRoot Configuration { get; }
        string SubscriptionNameA { get; }
        string SubscriptionNameB { get; }

        string FilterPropA { get; }

        string FilterPropB { get; }

        string TopicConnectionString { get; }
    }
}