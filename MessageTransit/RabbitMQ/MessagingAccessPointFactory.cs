using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessageTransit.RabbitMQ
{
    public class MessagingAccessPointFactory
    {
        private static MTRabbitMQConfig config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).Build().GetSection("MTRabbitMQ").Get<MTRabbitMQConfig>();
        private static ConcurrentDictionary<string, MessagingAccessPoint> accessPointContainer = new ConcurrentDictionary<string, MessagingAccessPoint>();
        public static MessagingAccessPoint getMessagingAccessPoint()
        {
            if(accessPointContainer.TryGetValue(config.MTConnnectionString,out MessagingAccessPoint accessPoint))
            {
                return accessPoint;
            }
            else
            {
                accessPoint = MessagingAccessPointAdapter.getMessagingAccessPoint(config.MTConnnectionString, new Dictionary<string, string>());
                accessPoint.setRetryStrategy(new RabbitMQRetryStrategy()
                {
                    RetryInterval = config.RetryInterval.GetValueOrDefault(),
                    RetryCount = config.RetryCount.GetValueOrDefault()
                });
                accessPoint.startup();
                accessPointContainer.TryAdd(config.MTConnnectionString, accessPoint);
                return accessPoint;
            }
        }
    }

    public class MTRabbitMQConfig
    {
        public string MTConnnectionString { get; set; }
        public int? RetryInterval { get; set; }
        public int? RetryCount { get; set; }
    }
}
