using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit.RabbitMQ
{
    public class MTRabbitMQConfig
    {
        public string MTConnnectionString { get; set; }
        public int? RetryInterval { get; set; }
        public int? RetryCount { get; set; }
    }
}
