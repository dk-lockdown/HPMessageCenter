using MessageTransit.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter
{
    public interface IProducerContainer
    {
        bool Send(IMessage message);
    }
}
