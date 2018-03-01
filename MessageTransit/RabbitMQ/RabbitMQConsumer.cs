using MessageTransit.Consumer;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using MessageTransit.Message;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using Newtonsoft.Json;
using MessageTransit.Monitor;

namespace MessageTransit.RabbitMQ
{
    public class RabbitMQConsumer : IConsumer
    {
        private IConnection connection;
        private Dictionary<string, string> consumerProperties;
        private IModel channel;
        private List<IMonitor> monitors;
        private IProcessor processor;
        private Stopwatch stopwatch = new Stopwatch();

        public RabbitMQConsumer(IConnection connection, Dictionary<string, string> properties)
        {
            this.connection = connection;
            this.consumerProperties = properties;
        }

        public Dictionary<string, string> properties()
        {
            return this.consumerProperties;
        }

        public void setProcessor(IProcessor processor)
        {
            this.processor = processor;
        }

        public void resume()
        {
            channel = connection.CreateModel();
            channel.ExchangeDeclare(consumerProperties[BuiltinKeys.Exchange], "fanout", durable: true, autoDelete: false, arguments: null);
            if (consumerProperties.TryGetValue(RabbitMQConst.TDX, out string tdx_value))
            {
                Dictionary<string, object> argumentsDelay = new Dictionary<string, object>();
                argumentsDelay.Add("x-dead-letter-exchange", tdx_value);
                channel.QueueDeclare(consumerProperties[BuiltinKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: argumentsDelay);
            }
            else
            {
                channel.QueueDeclare(consumerProperties[BuiltinKeys.Topic], durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
            channel.QueueBind(consumerProperties[BuiltinKeys.Topic], consumerProperties[BuiltinKeys.Exchange], string.Empty);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicQos(0, 1, false);
            channel.BasicConsume(consumerProperties[BuiltinKeys.Topic], false, consumer);
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var msgStr = Encoding.UTF8.GetString(body);
            IMessage message = JsonConvert.DeserializeObject<TextMessage>(msgStr);
            if (processor == null)
                throw new MessageTransitException("Processor can not be null!");
            try
            {
                stopwatch.Restart();
                bool result = (processor.Process(message)).Result;
                stopwatch.Stop();

                if(result)
                {
                    channel.BasicAck(e.DeliveryTag,false);
                    if(monitors!=null&&monitors.Count>0)
                    {
                        MessageSuccessEventArgs args = new MessageSuccessEventArgs(message, stopwatch.ElapsedMilliseconds);
                        monitors.ForEach(m => m.onEvent(args));
                    }
                }
                else
                {
                    channel.BasicReject(e.DeliveryTag, false);
                    if (monitors != null && monitors.Count > 0)
                    {
                        MessageExceptionEventArgs args = new MessageExceptionEventArgs(message, new Exception("Proccess Failed!"), stopwatch.ElapsedMilliseconds);
                        monitors.ForEach(m => m.onEvent(args));
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                channel.BasicReject(e.DeliveryTag, false);
                if (monitors != null && monitors.Count > 0)
                {
                    MessageExceptionEventArgs args = new MessageExceptionEventArgs(message, ex,stopwatch.ElapsedMilliseconds);
                    monitors.ForEach(m => m.onEvent(args));
                }
            }
        }

        public void addMonitor(IMonitor monitor)
        {
            if(monitors==null)
            {
                monitors = new List<IMonitor>();
            }
            monitors.Add(monitor);
        }

        public void removeMonitor(IMonitor monitor)
        {
            monitors.Remove(monitor);
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
