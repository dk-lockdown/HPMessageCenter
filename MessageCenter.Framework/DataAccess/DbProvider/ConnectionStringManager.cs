using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageCenter.DataAccess
{
    public static class ConnectionStringManager
    {
        internal class ConnStrSetting
        {
            private string m_Name;
            private string m_ConnectionString;
            private ProviderType m_ProviderType;
            public string Name
            {
                get
                {
                    return this.m_Name;
                }
            }
            public string ConnectionString
            {
                get
                {
                    return this.m_ConnectionString;
                }
            }
            public ProviderType ProviderType
            {
                get
                {
                    return this.m_ProviderType;
                }
            }
            public ConnStrSetting(string name, string connStr, string providerName)
                : this(name, connStr, ConnectionStringManager.ConvertProviderNameToType(providerName, name, connStr))
            {
            }
            public ConnStrSetting(string name, string connStr, ProviderType proType)
            {
                this.m_Name = name;
                this.m_ConnectionString = connStr;
                this.m_ProviderType = proType;
            }
        }
        private static Dictionary<string, ConnectionStringManager.ConnStrSetting> s_ConnStrMaps = new Dictionary<string, ConnectionStringManager.ConnStrSetting>();
        private static Func<string, ConnectionStringManager.ConnStrSetting> s_ConnStrGetter = null;
        private static object s_SyncObject = new object();
        public static int ConnectionStringCount
        {
            get
            {
                int result;
                if (ConnectionStringManager.s_ConnStrMaps == null)
                {
                    result = 0;
                }
                else
                {
                    result = ConnectionStringManager.s_ConnStrMaps.Count;
                }
                return result;
            }
        }
        internal static void GetConnectionInfo(string databaseName, out string connectionString, out ProviderType providerType)
        {
            ConnectionStringManager.ConnStrSetting connStrSetting;
            if (!ConnectionStringManager.s_ConnStrMaps.TryGetValue(databaseName, out connStrSetting) || connStrSetting == null)
            {
                Func<string, ConnectionStringManager.ConnStrSetting> func = ConnectionStringManager.s_ConnStrGetter;
                connStrSetting = ((func != null) ? func(databaseName) : null);
            }
            if (connStrSetting != null)
            {
                connectionString = connStrSetting.ConnectionString;
                providerType = connStrSetting.ProviderType;
                return;
            }
            throw new NotSupportedException("Can't find the info of database '" + databaseName + "'. It hasn't been configurated.");
        }
        internal static void ClearAllConnectionString()
        {
            if (ConnectionStringManager.s_ConnStrMaps != null)
            {
                ConnectionStringManager.s_ConnStrMaps.Clear();
            }
        }
        public static void SetConnectionString(string databaseName, string connStr, string providerName)
        {
            ProviderType providerType = ConnectionStringManager.ConvertProviderNameToType(providerName, databaseName, connStr);
            ConnectionStringManager.SetConnectionString(databaseName, connStr, providerType);
        }
        public static void SetConnectionString(string databaseName, string connStr, ProviderType providerType)
        {
            ConnectionStringManager.ConnStrSetting value = new ConnectionStringManager.ConnStrSetting(databaseName, connStr, providerType);
            if (ConnectionStringManager.s_ConnStrMaps.ContainsKey(databaseName))
            {
                ConnectionStringManager.s_ConnStrMaps[databaseName] = value;
            }
            else
            {
                ConnectionStringManager.s_ConnStrMaps.Add(databaseName, value);
            }
        }
        internal static void SetConnectionString(Func<string, ConnectionStringManager.ConnStrSetting> func)
        {
            ConnectionStringManager.s_ConnStrGetter = func;
        }
        public static void SetSqlServerConnectionString(string databaseName, string connStr)
        {
            ConnectionStringManager.SetConnectionString(databaseName, connStr, ProviderType.SqlServer);
        }
        private static ProviderType ConvertProviderNameToType(string providerName, string databaseName, string connStr)
        {
            ProviderType result;
            if (providerName != null && providerName.Trim().Length > 0)
            {
                string text = providerName.Trim().ToLower();
                string text2 = text;
                if (text2 != null)
                {
                    if (text2 == "sqlserver")
                    {
                        result = ProviderType.SqlServer;
                        return result;
                    }
                    if (text2 == "odbc")
                    {
                        result = ProviderType.Odbc;
                        return result;
                    }
                    if (text2 == "oledb")
                    {
                        result = ProviderType.OleDb;
                        return result;
                    }
                }
                throw new ConfigurationErrorsException(string.Concat(new string[]
				{
					"Not support this database provider '",
					providerName,
					"' for database whose name is '",
					databaseName,
					"' and connection string is '",
					connStr,
					"'."
				}));
            }
            result = ProviderType.SqlServer;
            return result;
        }
    }

    public class ConfigurationErrorsException : Exception
    {
        public ConfigurationErrorsException(string message)
            : base(message)
        {
        }
        public ConfigurationErrorsException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
