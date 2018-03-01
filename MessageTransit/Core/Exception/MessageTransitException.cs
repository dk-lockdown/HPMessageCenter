using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit
{
    public class MessageTransitException:Exception
    {
        public MessageTransitException(string message)
            :base(message)
        {

        }
    }
}
