using MessageTransit.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit.Integration
{
    public interface IProducerContainer
    {
        bool Send(IMessage message);
    }
}
