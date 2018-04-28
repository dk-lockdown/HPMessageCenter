using MessageTransit;
using MessageTransit.Producer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using MessageTransit.Message;
using Microsoft.Extensions.Configuration;
using System.IO;
using MessageTransit.RabbitMQ;

namespace MessageCenter
{
    public class RabbitMQProducerContainer:IProducerContainer
    {
        private static ConcurrentDictionary<string, IProducer> producerContainer = new ConcurrentDictionary<string, IProducer>();

        public bool Send(IMessage message)
        {
            IProducer producer;
            if (!producerContainer.ContainsKey(message.Headers[BuiltinKeys.Topic]))
            {
                MessagingAccessPoint accessPoint =MessagingAccessPointFactory.getMessagingAccessPoint();
                Dictionary<string, string> properties = new Dictionary<string, string>();
                if (message.Headers.TryGetValue(BuiltinKeys.Exchange, out string exchange))
                {
                    properties.Add(BuiltinKeys.Exchange, exchange);
                }
                if (message.Headers.TryGetValue(BuiltinKeys.Topic, out string topic))
                {
                    properties.Add(BuiltinKeys.Topic, topic);
                }
                producer = accessPoint.createProducer(properties);
                producer.startup();
                producerContainer.TryAdd(message.Headers[BuiltinKeys.Topic], producer);
            }
            else
            {
                producer = producerContainer[message.Headers[BuiltinKeys.Topic]];
            }
            return producer.Send(message);
        }
    }
}
