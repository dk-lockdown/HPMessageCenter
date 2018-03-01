#if NETSTANDARD1_3 || NETSTANDARD2_0
using MessageCenter.Framework.Extension;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MessageCenter.Framework.Log
{
    public static class Logger
    {
        private static string WriteLog(LogEntry log, List<ILogEmitter> logEmitterList)
        {
            if (logEmitterList == null || logEmitterList.Count <= 0)
            {
                return log.LogID.ToString();
            }
            try
            {
                foreach (var logEmitter in logEmitterList)
                {
                    if (logEmitter == null)
                    {
                        continue;
                    }
                    logEmitter.EmitLog(log);
                }
            }
            catch(Exception ex)
            {
                string message = string.Format("Write log failed.\r\n\r\n Error Info: {0}. \r\n\r\n Log Info: {1}", ex.ToString(), log.SerializationWithoutException());
                return message;
            }

            return log.LogID.ToString();
        }

        private static string WriteLog(LogEntry log)
        {
            List<ILogEmitter> logEmitterList;
            try
            {
                logEmitterList = EmitterFactory.Create();
            }
            catch(Exception ex)
            {
                string message = string.Format("Failed to create log emitter instance.\r\n\r\n Error Info: {0}. \r\n\r\n Log Info: {1}", ex.ToString(), log.SerializationWithoutException());
                return message;
            }
            return WriteLog(log, logEmitterList);
        }

        public static string WriteLog(string content, string category = null, string referenceKey = null, List<KeyValuePair<string, object>> extendedProperties = null)
        {
            LogEntry log = new LogEntry();
            log.ServerTime = DateTime.Now;
            log.LogID = Guid.NewGuid();
            log.Source = GetSource();
            log.RequestUrl = GetRequestUrl();
            log.UserHostName = GetUserHostName();
            log.UserHostAddress = GetUserHostAddress();
            log.ServerIP = GetServerIP().Result;
            try
            {
                log.ServerName = Dns.GetHostName();
            }
            catch { }
            string p_name;
            log.ProcessID = GetProcessInfo(out p_name);
            log.ProcessName = p_name;
            try
            {
                log.ThreadID = Thread.CurrentThread.ManagedThreadId;
            }
            catch { }

            log.Category = category;
            log.Content = content;
            log.ReferenceKey = referenceKey;
            if (extendedProperties != null && extendedProperties.Count > 0)
            {
                foreach (var p in extendedProperties)
                {
                    log.AddExtendedProperty(p.Key, p.Value);
                }
            }

            return WriteLog(log);
        }

        private static string GetSource()
        {
            try
            {
                LogSetting s = LogSection.GetSetting();
                if (s != null)
                {
                    return s.Source;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetRequestUrl()
        {
            try
            {
                if (NetCoreHttpContext.Current != null && NetCoreHttpContext.Current.Request != null)
                {
                    return NetCoreHttpContext.Current.Request.Path;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetUserHostName()
        {
            try
            {
                if (NetCoreHttpContext.Current != null && NetCoreHttpContext.Current.Request != null)
                {
                    return NetCoreHttpContext.Current.Request.Host.Host;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetUserHostAddress()
        {
            try
            {
                if (NetCoreHttpContext.Current != null && NetCoreHttpContext.Current.Request != null)
                {
                    return !StringValues.IsNullOrEmpty(NetCoreHttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"])?NetCoreHttpContext.Current.Request.Headers["HTTP_X_FORWARDED_FOR"]: NetCoreHttpContext.Current.Request.Headers["REMOTE_ADDR"];
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string s_ServerIP;
        private static async Task<string> GetServerIP()
        {
            if (string.IsNullOrEmpty(s_ServerIP))
            {
                try
                {
                    IPAddress[] address = await Dns.GetHostAddressesAsync(Dns.GetHostName());
                    if (address != null)
                    {
                        foreach (IPAddress addr in address)
                        {
                            if (addr == null)
                            {
                                continue;
                            }
                            string tmp = addr.ToString().Trim();
                            //过滤IPv6的地址信息
                            if (tmp.Length <= 16 && tmp.Length > 5)
                            {
                                s_ServerIP = tmp;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    //s_ServerIP = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(s_ServerIP))
            {
                return string.Empty;
            }
            return s_ServerIP;
        }

        private static int GetProcessInfo(out string name)
        {
            try
            {
                Process p = Process.GetCurrentProcess();
                if (p == null)
                {
                    name = null;
                    return -1;
                }
                name = p.ProcessName;
                return p.Id;
            }
            catch
            {
                name = string.Empty;
                return -1;
            }
        }
    }
}
#endif