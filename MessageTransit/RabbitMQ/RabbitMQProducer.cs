using MessageTransit.Producer;
using System;
using System.Collections.Generic;
using System.Text;
using MessageTransit.Message;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace MessageTransit.RabbitMQ
{
    public class RabbitMQProducer : IProducer
    {
        private IConnection connection;
        private Dictionary<string, string> producerProperties;
        private IModel channel;

        public RabbitMQProducer(IConnection connection, Dictionary<string, string> properties)
        {
            this.connection = connection;
            this.producerProperties = properties;

            channel = connection.CreateModel();
            channel.ExchangeDeclare(producerProperties[BuiltinKeys.Exchange], "fanout", durable: true, autoDelete: false, arguments: null);
            if (producerProperties.TryGetValue(RabbitMQConst.TDX, out string tdx_value))
            {
                Dictionary<string, object> argumentsDelay = new Dictionary<string, object>();
                argumentsDelay.Add("x-dead-letter-exchange", tdx_value);
                channel.QueueDeclare(producerProperties[BuiltinKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: argumentsDelay);
            }
            else
            {
                channel.QueueDeclare(producerProperties[BuiltinKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
            channel.QueueBind(producerProperties[BuiltinKeys.Topic], producerProperties[BuiltinKeys.Exchange], string.Empty);
        }

        public Dictionary<string, string> properties()
        {
            return producerProperties;
        }

        public bool Send(IMessage message)
        {
            var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            channel.ConfirmSelect();
            channel.BasicPublish(message.Headers[BuiltinKeys.Exchange], "", props, msgBody);
            return channel.WaitForConfirms();
        }

        public void shutdown()
        {
            channel.Close();
            channel.Dispose();
        }

        public void startup()
        {
            
        }
    }
}
