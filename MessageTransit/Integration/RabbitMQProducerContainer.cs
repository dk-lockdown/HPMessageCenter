using MessageTransit.Producer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using MessageTransit.Message;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MessageTransit.Integration
{
    public class RabbitMQProducerContainer:IProducerContainer
    {
        private IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).Build();
        private static ConcurrentDictionary<string, IProducer> producerContainer = new ConcurrentDictionary<string, IProducer>();

        public bool Send(IMessage message)
        {
            IProducer producer;
            if (!producerContainer.ContainsKey(message.Headers[BuiltinKeys.Topic]))
            {
                MessagingAccessPoint accessPoint =MessagingAccessPointFactory.getMessagingAccessPoint(configuration.GetConnectionString("MTConnectionString"));
                producer = accessPoint.createProducer(message.Headers);
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
