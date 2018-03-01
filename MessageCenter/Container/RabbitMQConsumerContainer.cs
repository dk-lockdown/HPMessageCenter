using MessageTransit;
using MessageTransit.Consumer;
using MessageTransit.Message;
using MessageTransit.Monitor;
using MessageTransit.RabbitMQ;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessageCenter
{
    public class RabbitMQConsumerContainer : IConsumerContainer
    {
        private static ConcurrentDictionary<string, IConsumer> consumerContainer = new ConcurrentDictionary<string, IConsumer>();
        private IMonitor monitor;

        public RabbitMQConsumerContainer(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        public void Add(string exchange, string topic)
        {
            if (!consumerContainer.ContainsKey(topic))
            {
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties.Add(BuiltinKeys.Exchange, exchange);
                properties.Add(BuiltinKeys.Topic, topic);
                MessagingAccessPoint accessPoint = MessagingAccessPointFactory.getMessagingAccessPoint();
                IConsumer consumer = accessPoint.createConsumer(properties);
                consumer.setProcessor(new RestApiProcessor());
                if (monitor != null)
                {
                    consumer.addMonitor(monitor);
                }
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
