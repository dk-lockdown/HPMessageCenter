#if NETSTANDARD1_3 || NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MessageCenter.Framework.Log
{
    internal class LogSection
    {
        private static LogSetting s_LogSetting = null;
        private static bool s_HasInit = false;
        private static object s_SyncObj = new object();
        public static LogSetting GetSetting()
        {
            if (s_HasInit == false)
            {
                lock (s_SyncObj)
                {
                    if (s_HasInit == false)
                    {
                        s_LogSetting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).Build().GetSection("LogSetting").Get<LogSetting>();
                        s_HasInit = true;
                    }
                }
            }
            return s_LogSetting;
        }
    }

    public class LogSetting
    {
        public List<LogEmitterConfig> Emitters { get; set; }

        public string Source { get; set; }
    }

    public class LogEmitterConfig
    {
        public string Type { get; set; }
        public Dictionary<string, string> Parameters
        {
            get;
            set;
        }
    }   
}
#endif
