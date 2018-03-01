using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit
{
    public interface IRetryStrategy
    {
        int RetryInterval { get; set; }

        int RetryCount { get; set; }
    }
}
