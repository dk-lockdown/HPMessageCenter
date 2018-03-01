#if NETSTANDARD1_3 || NETSTANDARD2_0
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MessageCenter.Framework.Log
{
    internal static class LogEntryExtend
    {
        public static void AddExtendedProperty(this LogEntry log, string propertyName, object propertyValue)
        {
            if (log == null)
            {
                return;
            }
            if (log.ExtendedProperties == null)
            {
                log.ExtendedProperties = new List<ExtendedPropertyData>();
            }
            log.ExtendedProperties.Add(new ExtendedPropertyData() { PropertyName = propertyName, PropertyValue = SerializePropertyValue(propertyValue) });
        }

        public static string SerializationWithoutException(this LogEntry log)
        {
            if (log == null)
            {
                return string.Empty;
            }
            StringBuilder manualSerialized = new StringBuilder();
            manualSerialized.AppendFormat("\r\n[LogID]: {0}", log.LogID);
            manualSerialized.AppendFormat("\r\n[Source]: {0}", log.Source);
            if (log.Category != null && log.Category.Trim().Length > 0)
            {
                manualSerialized.AppendFormat("\r\n[Category]: {0}", log.Category);
            }
            if (log.RequestUrl != null && log.RequestUrl.Trim().Length > 0)
            {
                manualSerialized.AppendFormat("\r\n[RequestUrl]: {0}", log.RequestUrl);
            }
            if (log.UserHostName != null && log.UserHostName.Trim().Length > 0)
            {
                manualSerialized.AppendFormat("\r\n[UserHostName]: {0}", log.UserHostName);
            }
            if (log.UserHostAddress != null && log.UserHostAddress.Trim().Length > 0)
            {
                manualSerialized.AppendFormat("\r\n[UserHostAddress]: {0}", log.UserHostAddress);
            }
            manualSerialized.AppendFormat("\r\n[Content]: {0}", log.Content);
            manualSerialized.AppendFormat("\r\n[ServerIP]: {0}", log.ServerIP);
            manualSerialized.AppendFormat("\r\n[ServerName]: {0}", log.ServerName);
            manualSerialized.AppendFormat("\r\n[ServerTime]: {0}", log.ServerTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            manualSerialized.AppendFormat("\r\n[ProcessID]: {0}", log.ProcessID);
            manualSerialized.AppendFormat("\r\n[ProcessName]: {0}", log.ProcessName);
            manualSerialized.AppendFormat("\r\n[ThreadID]: {0}", log.ThreadID);
            if (log.ReferenceKey != null && log.ReferenceKey.Trim().Length > 0)
            {
                manualSerialized.AppendFormat("\r\n[ReferenceKey]: {0}", log.ReferenceKey);
            }
            if (log.ExtendedProperties != null && log.ExtendedProperties.Count > 0)
            {
                manualSerialized.Append("\r\n[ExtendedProperties]:");
                manualSerialized.Append(log.ExtendedProperties.Serialize());
            }
            return manualSerialized.ToString();
        }

        public static string Serialize(this List<ExtendedPropertyData> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pro in list)
            {
                if (pro == null)
                {
                    continue;
                }
                sb.AppendFormat("\r\n {0}: {1}", pro.PropertyName, pro.PropertyValue);
            }
            return sb.ToString();
        }

        private static string SerializePropertyValue(object propertyValue)
        {
            if (propertyValue == null)
            {
                return "NULL";
            }
            else if (propertyValue is string)
            {
                return "[String] " + propertyValue.ToString().Trim();
            }
            else if (propertyValue.GetType().GetTypeInfo().IsPrimitive)
            {
                return "[" + propertyValue.GetType().Name + "] " + propertyValue.ToString();
            }
            else
            {
                try
                {
                    Type type = propertyValue.GetType();
                    string name = type.FullName;
                    if (type.FullName.Contains("<>f__AnonymousType"))
                    {
                        name = "AnonymousType";
                    }
                    return "[" + name + "] " + JsonConvert.SerializeObject(propertyValue);
                }
                catch (Exception ex)
                {
                    return "Serialize exception for type '" + propertyValue.GetType().AssemblyQualifiedName + "' : " + ex.Message;
                }
            }
        }
    }
}
#endif