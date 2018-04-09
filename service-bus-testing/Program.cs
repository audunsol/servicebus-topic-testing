using System;
using System.Threading.Tasks;

namespace service_bus_testing
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            var config = new Config();

            var sender = new SenderService(config);
            var subscriptionA = new Subscriber(config, config.SubscriptionNameA, config.FilterPropA);
            var subscriptionB = new Subscriber(config, config.SubscriptionNameB, config.FilterPropB);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Send messages:
            await sender.SendMessagesAsync(10);

            Console.ReadKey();

            await sender.CloseAsync();
            await subscriptionA.CloseAsync();
            await subscriptionB.CloseAsync();
        }
    }
}
