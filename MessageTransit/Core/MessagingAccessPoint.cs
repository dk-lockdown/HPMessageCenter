using MessageTransit.Consumer;
using MessageTransit.Logger;
using MessageTransit.Monitor;
using MessageTransit.Producer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit
{
    public interface MessagingAccessPoint:ServiceLifecycle
    {
        Dictionary<string, string> properties();

        void setRetryStrategy(IRetryStrategy retryStrategy);

        IRetryStrategy getRetryStrategy();

        void addLogger(ILogger logger);

        void removeLogger(ILogger logger);

        IProducer createProducer(Dictionary<string, string> properties);

        IConsumer createConsumer(Dictionary<string,string> properties);

        List<IProducer> producers();

        List<IConsumer> consumers();        
    }
}
