using Microsoft.Azure.ServiceBus;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace service_bus_testing
{
    internal class Subscriber
    {
        private readonly Config _config;
        private readonly string _subscriptionName;
        private readonly string _filterProperty;
        private readonly ISubscriptionClient _subscriptionClient;

        public Subscriber(Config config, string subscriptionName, string filterProperty)
        {
            _config = config;
            _subscriptionName = subscriptionName;
            _filterProperty = filterProperty;

            var csBuilder = new ServiceBusConnectionStringBuilder(_config.TopicConnectionString);

            _subscriptionClient = new SubscriptionClient(csBuilder, subscriptionName);

            SetupSubscriptionRules().GetAwaiter().GetResult();

            // Register subscription message handler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public async Task SetupSubscriptionRules()
        {
            var subscriptionRuleName = _subscriptionName + "-rule";
            var newRule = CreateNewRuleDescription(subscriptionRuleName);

            var exitstingRules = await _subscriptionClient.GetRulesAsync();


            if (exitstingRules != null)
            {
                exitstingRules
                    .Where(r => !CompareRules(r, newRule))
                    .ToList()
                    .ForEach(async r => await _subscriptionClient.RemoveRuleAsync(r.Name));

                if (exitstingRules.Any(r => CompareRules(r, newRule)))
                {
                    return;
                }
            }

            await _subscriptionClient.AddRuleAsync(newRule);
        }

        private bool CompareRules(RuleDescription oldRule, RuleDescription newRule)
        {
            return oldRule.Name == newRule.Name && oldRule.Filter.ToString() == newRule.Filter.ToString();
        }

        private RuleDescription CreateNewRuleDescription(string subscriptionRuleName)
        {
            var corrFilter = new CorrelationFilter();
            corrFilter.Properties.Add(_filterProperty, "true");
            var rule = new RuleDescription
            {
                Filter = corrFilter,
                Name = subscriptionRuleName
            };
            return rule;
        }

        public async Task CloseAsync()
        {
            await _subscriptionClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            Console.WriteLine($"{_subscriptionName}: Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the subscriptionClient has already been closed.
            // If subscriptionClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        // Use this handler to examine the exceptions received on the message pump.
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}