using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MessageTransit
{
    public class AccessPointURI
    {
        private const string pattern = "^.+://.+[/|?]{0,1}.+$";
        private string driverType;
        private string hostAddress;
        private List<KeyValuePair<string, string>> properties;

        public AccessPointURI(string connectionString)
        {
            validateConnectionString(connectionString);
            int idx = connectionString.IndexOf("://");
            this.driverType = connectionString.Substring(0, idx);

            string unprocessedString = connectionString.Substring(driverType.Length + 3);
            idx = unprocessedString.LastIndexOfAny(new char[] { '/', '?' });
            if (idx > 0)
            {
                this.hostAddress = unprocessedString.Substring(0, idx);

                if (unprocessedString.Length > hostAddress.Length + 1)
                {
                    unprocessedString = unprocessedString.Substring(hostAddress.Length + 1);
                    properties = new List<KeyValuePair<string, string>>();
                    string[] props = unprocessedString.Split('&', StringSplitOptions.RemoveEmptyEntries);
                    foreach(var prop in props)
                    {
                        string[] kv = prop.Split('=');
                        
                        if(kv.Length==2)
                        {
                            KeyValuePair<string, string> p = new KeyValuePair<string, string>(kv[0].ToLower(), kv[1]);
                            properties.Add(p);
                        }                        
                    }
                }
            }
            else
            {
                this.hostAddress = unprocessedString;
            }
        }
         
        public string getDriverType()
        {
            return driverType;
        }
        public string getHostAddress()
        {
            return hostAddress;
        }

        public List<KeyValuePair<string, string>> getProperties()
        {
            return properties;
        }

        private void validateConnectionString(string connectionString)
        {
            Regex reg = new Regex(pattern);
            if (!reg.IsMatch(connectionString))
            {
                throw new ArgumentException("MQ connection string is not valid!");
            }
        }
    }
}
