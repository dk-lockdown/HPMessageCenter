using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter
{
    public class MessageTransitConfig
    {
        public string MTConnnectionString { get; set; }
        public int? RetryInterval { get; set; }
        public int? RetryCount { get; set; }
    }
}
