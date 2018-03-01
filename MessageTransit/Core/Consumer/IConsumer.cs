using MessageTransit.Monitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit.Consumer
{
    public interface IConsumer: ServiceLifecycle
    {
        Dictionary<string, string> properties();

        void setProcessor(IProcessor processor);

        void resume();

        void addMonitor(IMonitor monitor);

        void removeMonitor(IMonitor monitor);
    }
}
