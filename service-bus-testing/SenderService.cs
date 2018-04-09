using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace service_bus_testing
{
    public class SenderService
    {
        private readonly TopicClient _topicClient;
        private readonly IConfig _config;

        public SenderService(IConfig config)
        {
            _config = config;
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(config.TopicConnectionString);
            _topicClient = new TopicClient(connectionStringBuilder);
        }

        public async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    if (i % 2 == 0)
                    {
                        message.UserProperties.Add(_config.FilterPropA, true);
                    }
                    else
                    {
                        message.UserProperties.Add(_config.FilterPropB, true);
                    }

                    if (i % 4 == 0)
                    {
                        message.UserProperties.Add(_config.FilterPropB, true);
                    }

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the topic.
                    await _topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        public async Task CloseAsync()
        {
            await _topicClient.CloseAsync();
        }
    }
}
