using MessageTransit.Consumer;
using MessageTransit.Message;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessageTransit.Integration
{
    public class RabbitConsumerContainer : IConsumerContainer
    {
        private IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).Build();
        private static ConcurrentDictionary<string, IConsumer> consumerContainer = new ConcurrentDictionary<string, IConsumer>();

        public void Add(string exchange, string topic)
        {
            if (!consumerContainer.ContainsKey(topic))
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties.Add(BuiltinKeys.Exchange, exchange);
                properties.Add(BuiltinKeys.Topic, topic);
                MessagingAccessPoint accessPoint = MessagingAccessPointFactory.getMessagingAccessPoint(configuration.GetConnectionString("MTConnectionString"));
                IConsumer consumer = accessPoint.createConsumer(properties);
                consumer.startup();
                consumer.resume();
                consumerContainer.TryAdd(topic, consumer);
            }
        }

        public void Remove(string exchange, string topic)
        {
            consumerContainer.TryRemove(topic, out IConsumer consumer);
            consumer.shutdown();
        }
    }
}
