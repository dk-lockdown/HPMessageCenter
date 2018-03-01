using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit
{
    public interface ServiceLifecycle
    {
        void startup();
        
        void shutdown();
    }
}
