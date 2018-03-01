using MessageTransit.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit.Producer
{
    public interface IProducer: ServiceLifecycle
    {
        Dictionary<string, string> properties();

        bool Send(IMessage message);
    }
}
