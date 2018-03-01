using MessageTransit;
using MessageTransit.Logger;
using MessageTransit.Monitor;
using MessageTransit.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter
{
    public class Startup
    {
        private static IProducerContainer producerContainer;
        private static IConsumerContainer consumerContainer;

        public static IProducerContainer ProducerContainer
        {
            get
            {
                if (producerContainer == null)
                {
                    throw new MessageTransitException("Startup not init!");
                }
                return producerContainer;
            }
        }

        public static IConsumerContainer ConsumerContainer
        {
            get
            {
                if (consumerContainer == null)
                {
                    throw new MessageTransitException("Startup not init!");
                }
                return consumerContainer;
            }
        }

        public static void Init(IMonitor _monitor,ILogger logger=null)
        {
            producerContainer = new RabbitMQProducerContainer();
            consumerContainer = new RabbitMQConsumerContainer(_monitor);
            if(logger!=null)
            {
                MessagingAccessPointFactory.getMessagingAccessPoint().addLogger(logger);
            }
        }

        public static void Init(IProducerContainer _producerContainer, IConsumerContainer _consumerContainer, ILogger logger = null)
        {            
            producerContainer = _producerContainer;
            consumerContainer = _consumerContainer;
            if (logger != null)
            {
                MessagingAccessPointFactory.getMessagingAccessPoint().addLogger(logger);
            }
        }
    }
}
