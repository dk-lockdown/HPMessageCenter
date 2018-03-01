using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit.Logger
{
    public interface ILogger
    {
        void error(string message, Exception ex);
        void info(string message);
    }
}
