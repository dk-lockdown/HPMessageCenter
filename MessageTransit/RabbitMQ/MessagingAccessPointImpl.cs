using System;
using System.Collections.Generic;
using System.Text;
using MessageTransit.Consumer;
using MessageTransit.Monitor;
using MessageTransit.Producer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MessageTransit.Message;
using Newtonsoft.Json;
using MessageTransit.Logger;

namespace MessageTransit.RabbitMQ
{
    public class MessagingAccessPointImpl : MessagingAccessPoint
    {
        private static object sync_obj = new object();
        private RabbitMQConfig s_Setting;
        private IConnection connection;
        private Dictionary<string, string> accessPointProperties;
        private IRetryStrategy retryStrategy;
        private List<ILogger> loggers;

        public MessagingAccessPointImpl(Dictionary<string, string> accessPointProperties)
        {
            this.accessPointProperties = accessPointProperties;
            if (connection != null) return;
            lock (sync_obj)
            {
                s_Setting = populate(this.accessPointProperties);
                var ap = s_Setting.HostAddress.Split(':', StringSplitOptions.RemoveEmptyEntries);
                string hostAddress = ap[0];int port = ap.Length == 2 ? int.Parse(ap[1]) : 5672;
                var factory = new ConnectionFactory
                {
                    Port= port,
                    HostName = hostAddress,
                    UserName = s_Setting.UserName,
                    Password = s_Setting.Password,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = new TimeSpan(s_Setting.NetworkRecoveryInterval ?? 1000),
                };
                connection = connection ?? factory.CreateConnection();
                connection.ConnectionShutdown += Connection_ConnectionShutdown;
            }
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            string message = $"{DateTime.Now.ToString("yyyy-mm-dd hh:MM:ss")} connection shutdown:{e.ReplyText}";
            Console.WriteLine(message);
            if (loggers != null&&loggers.Count>0)
            {
                loggers.ForEach(logger => logger.error(message, null));
            }
        }

        private static RabbitMQConfig populate(Dictionary<string, string> properties)
        {
            RabbitMQConfig config = new RabbitMQConfig();
            if (properties.TryGetValue("hostaddress", out string address))
            {
                config.HostAddress = address;
            }
            else
            {
                throw new ArgumentException("there is no property [address]!");
            }
            if (properties.TryGetValue("username", out string username))
            {
                config.UserName = username;
            }
            else
            {
                throw new ArgumentException("there is no property [username]!");
            }
            if (properties.TryGetValue("password", out string password))
            {
                config.Password = password;
            }
            else
            {
                throw new ArgumentException("there is no property [password]!");
            }
            if (properties.TryGetValue("networkrecoveryinterval", out string networkrecoveryinterval))
            {
                if (int.TryParse(networkrecoveryinterval, out int interval))
                {
                    config.NetworkRecoveryInterval = interval;
                }
                else
                {
                    throw new ArgumentException("property [networkrecoveryinterval] must be integer!");
                }
            }
            return config;
        }

        public List<IConsumer> consumers()
        {
            throw new NotImplementedException();
        }

        public IConsumer createConsumer(Dictionary<string, string> properties)
        {
            if (accessPointProperties.TryGetValue(RabbitMQConst.TDX, out string tdx_value))
            {
                properties.Add(RabbitMQConst.TDX, tdx_value);
            }
            if (accessPointProperties.TryGetValue(RabbitMQConst.RetryInterval, out string retry_interval))
            {
                properties.Add(RabbitMQConst.RetryInterval, retry_interval);
            }
            if (accessPointProperties.TryGetValue(RabbitMQConst.RetryCount, out string retry_count))
            {
                properties.Add(RabbitMQConst.RetryCount, retry_count);
            }
            return new RabbitMQConsumer(connection, properties);
        }

        public IProducer createProducer(Dictionary<string, string> properties)
        {
            if (accessPointProperties.TryGetValue(RabbitMQConst.TDX, out string tdx_value))
            {
                properties.Add(RabbitMQConst.TDX, tdx_value);
            }
            if (accessPointProperties.TryGetValue(RabbitMQConst.RetryInterval, out string retry_interval))
            {
                properties.Add(RabbitMQConst.RetryInterval, retry_interval);
            }
            if (accessPointProperties.TryGetValue(RabbitMQConst.RetryCount, out string retry_count))
            {
                properties.Add(RabbitMQConst.RetryCount, retry_count);
            }
            return new RabbitMQProducer(connection, properties);
        }

        public void setRetryStrategy(IRetryStrategy strategy)
        {
            this.retryStrategy = strategy;
            if (strategy != null && strategy.RetryInterval > 0 && strategy.RetryCount > 0)
            {
                accessPointProperties.Add(RabbitMQConst.RetryInterval, strategy.RetryInterval.ToString());
                accessPointProperties.Add(RabbitMQConst.RetryCount, strategy.RetryCount.ToString());

                channel = connection.CreateModel();
                //延时队列Exchange
                channel.ExchangeDeclare(RabbitMQConst.TDX_Value, "fanout", durable: true, autoDelete: false, arguments: null);
                //死信队列Exchange
                channel.ExchangeDeclare(RabbitMQConst.DLX_Value, "fanout", durable: true, autoDelete: false, arguments: null);

                //延时队列的死信队列Exchange是RabbitMQConst.DLX_Value。普通任务队列的死信队列Exchange是RabbitMQConst.TDX_Value。
                //即普通任务队列死信后，消息被投递到延时队列，延时队列的消息经过消息过期时间后，发生死信，再被投递到死信队列。
                //死信队列的消费者将消息重新投递到消息队列，并在消息头设置消息重试次数。
                Dictionary<string, object> argumentsDelay = new Dictionary<string, object>();
                argumentsDelay.Add("x-dead-letter-exchange", RabbitMQConst.DLX_Value);
                argumentsDelay.Add("x-message-ttl", strategy.RetryInterval);
                channel.QueueDeclare(RabbitMQConst.TDQ_Value, true, false, false, argumentsDelay);
                channel.QueueBind(RabbitMQConst.TDQ_Value, RabbitMQConst.TDX_Value, string.Empty);

                channel.QueueDeclare(RabbitMQConst.DLQ_Value, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(RabbitMQConst.DLQ_Value, RabbitMQConst.DLX_Value, string.Empty);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicQos(0, 1, false);
                channel.BasicConsume(RabbitMQConst.DLQ_Value, false, consumer);

                accessPointProperties.Add(RabbitMQConst.TDX, RabbitMQConst.TDX_Value);
                accessPointProperties.Add(RabbitMQConst.DLX, RabbitMQConst.DLX_Value);
            }
        }

        public IRetryStrategy getRetryStrategy()
        {
            return retryStrategy;
        }

        public void addLogger(ILogger logger)
        {
            if(loggers==null)
            {
                loggers = new List<ILogger>();
            }
            loggers.Add(logger);
        }

        public void removeLogger(ILogger logger)
        {
            loggers.Remove(logger);
        }

        private IModel channel;

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {            
            var body = e.Body;
            var msgStr = Encoding.UTF8.GetString(body);
            IMessage message = JsonConvert.DeserializeObject<TextMessage>(msgStr);

            if (accessPointProperties.TryGetValue(RabbitMQConst.RetryCount, out string retryCount) && int.TryParse(retryCount, out int retryC))
            {
                //重新发消息
                if (message.Headers.TryGetValue(BuiltinKeys.RetryNumbers, out string retryNumber) && int.TryParse(retryNumber, out int retryNo))
                {
                    if (retryNo < retryC)
                    {
                        retryNo = retryNo + 1;
                        message.Headers[BuiltinKeys.RetryNumbers] = retryNo.ToString();
                        var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                        var props = channel.CreateBasicProperties();
                        props.Persistent = true;
                        channel.ConfirmSelect();
                        channel.BasicPublish(message.Headers[BuiltinKeys.Exchange], "", props, msgBody);
                        channel.WaitForConfirms();
                    }
                }
                else
                {
                    message.putHeaders(BuiltinKeys.RetryNumbers, "1");
                    var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    var props = channel.CreateBasicProperties();
                    props.Persistent = true;
                    channel.ConfirmSelect();
                    channel.BasicPublish(message.Headers[BuiltinKeys.Exchange], "", props, msgBody);
                    channel.WaitForConfirms();
                }
            }
            channel.BasicAck(e.DeliveryTag, false);
        }

        public List<IProducer> producers()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> properties()
        {
            return accessPointProperties;
        }

        public void shutdown()
        {
           
        }

        public void startup()
        {
            
        }
        
    }

    public class RabbitMQConfig
    {
        /// <summary>
        /// rabbitmq://192.168.60.60:5672/
        /// </summary>
        public string HostAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 重连时间
        /// </summary>
        public int? NetworkRecoveryInterval { get; set; }
    }

    public class RabbitMQRetryStrategy : IRetryStrategy
    {
        public int RetryInterval { get; set; }
        public int RetryCount { get; set; }
    }

    public class RabbitMQConst
    {
        public const string TDX = "tdx";
        public const string DLX = "dlx";
        public const string TDX_Value = "mt-time-delay-exchange";
        public const string DLX_Value = "mt-dead-letter-exchange";
        public const string TDQ_Value = "mt-time-delay-queue";
        public const string DLQ_Value = "mt-dead-letter-queue";
        public const string RetryInterval = "mt-retryinterval";
        public const string RetryCount = "mt-retrycount";
    }
}
