using System;
using System.Collections.Generic;

namespace MessageCenter.Framework.Log
{
    public class LogEntry
    {
        
        public Guid LogID { get; set; }

        
        public string Source { get; set; }

        
        public string Category { get; set; }

        
        public string RequestUrl { get; set; }

        
        public string UserHostName { get; set; }

        
        public string UserHostAddress { get; set; }

        
        public string Content { get; set; }

        
        public string ServerIP { get; set; }

        
        public string ServerName { get; set; }

        
        public DateTime ServerTime { get; set; }

        
        public string ReferenceKey { get; set; }

        
        public int ProcessID { get; set; }

        
        public string ProcessName { get; set; }

        
        public int ThreadID { get; set; }

        
        public List<ExtendedPropertyData> ExtendedProperties { get; set; }
    }


    public class ExtendedPropertyData
    {
        
        public string PropertyName { get; set; }

        
        public string PropertyValue { get; set; }
    }
}