using System;
using System.Collections.Generic;
using System.Text;

namespace MessageTransit
{
    public class MessagingAccessPointAdapter
    {
        public static MessagingAccessPoint getMessagingAccessPoint(string connectionString, Dictionary<string, string> properties)
        {
            AccessPointURI uri = new AccessPointURI(connectionString);
            string driverImpl = parseDriverImpl(uri.getDriverType(), properties);
            properties.Add("driverimpl", driverImpl);
            properties.Add("hostaddress", uri.getHostAddress());
            if (uri.getProperties()!=null&&uri.getProperties().Count>0)
            {
                List<KeyValuePair<string, string>> props = uri.getProperties();
                props.ForEach(prop =>
                {
                    properties.Add(prop.Key, prop.Value);
                });
            }
            MessagingAccessPoint vendorImpl = Activator.CreateInstance(Type.GetType(driverImpl),args:properties) as MessagingAccessPoint;
            return vendorImpl;
        }

        private static String parseDriverImpl(string driverType, Dictionary<string, string> properties)
        {
            if (properties.ContainsKey("driverimpl"))
            {
                return properties["driverimpl"];
            }
            return $"MessageTransit.{driverType}.MessagingAccessPointImpl";
        } 
    }
}
