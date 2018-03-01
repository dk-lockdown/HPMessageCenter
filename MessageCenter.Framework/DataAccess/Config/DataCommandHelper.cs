#if NETSTANDARD1_3 || NETSTANDARD2_0
using MessageCenter.Framework.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessageCenter.DataAccess
{
    public static class DataCommandHelper
    {
        static DataCommandHelper()
        {
            ConnectionStringManager.SetConnectionString(new Func<string, ConnectionStringManager.ConnStrSetting>(DataCommandHelper.GetConnStrSetting));
        }
        private static List<DatabaseInstance> GetDatabaseList()
        {
            List<DatabaseInstance> result;
            if (ConfigHelper.DatabaseListFilePath == null || ConfigHelper.DatabaseListFilePath.Trim().Length <= 0 || !File.Exists(ConfigHelper.DatabaseListFilePath.Trim()))
            {
                result = null;
            }
            else
            {
                result = CacheManager.GetWithLocalCache<List<DatabaseInstance>>("DA_DataCommandManager.GetDatabaseList", delegate
                {
                    DatabaseList databaseList = ConfigHelper.LoadDatabaseListFile();
                    List<DatabaseInstance> result2;
                    if (databaseList != null && databaseList.DatabaseInstances != null && databaseList.DatabaseInstances.Length > 0)
                    {
                        List<DatabaseInstance> list = new List<DatabaseInstance>(databaseList.DatabaseInstances.Length);
                        DatabaseInstance[] databaseInstances = databaseList.DatabaseInstances;
                        for (int i = 0; i < databaseInstances.Length; i++)
                        {
                            DatabaseInstance db = databaseInstances[i];
                            if (db != null && !string.IsNullOrWhiteSpace(db.Name) && !string.IsNullOrWhiteSpace(db.ConnectionString))
                            {
                                if (list.Exists((DatabaseInstance x) => x.Name == db.Name.Trim()))
                                {
                                    throw new FileLoadException(string.Concat(new string[]
                                    {
                                        "Duplidated database name '",
                                        db.Name,
                                        "' in configuration file '",
                                        ConfigHelper.DatabaseListFilePath,
                                        "'."
                                    }));
                                }
                                list.Add(new DatabaseInstance
                                {
                                    Name = db.Name.Trim(),
                                    ConnectionString = db.ConnectionString,
                                    Type = db.Type
                                });
                            }
                        }
                        result2 = list;
                    }
                    else
                    {
                        result2 = null;
                    }
                    return result2;
                });
            }
            return result;
        }
        private static ConnectionStringManager.ConnStrSetting GetConnStrSetting(string name)
        {
            List<DatabaseInstance> databaseList = GetDatabaseList();
            ConnectionStringManager.ConnStrSetting result;
            if (databaseList == null || databaseList.Count <= 0)
            {
                result = null;
            }
            else
            {
                DatabaseInstance databaseInstance = databaseList.Find((DatabaseInstance x) => x.Name == name);
                if (databaseInstance != null)
                {
                    result = new ConnectionStringManager.ConnStrSetting(databaseInstance.Name, databaseInstance.ConnectionString, databaseInstance.Type);
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }
        private static Dictionary<string, DataCommandConfig> GetAllDataCommandConfigInfosFromCache()
        {
            Dictionary<string, DataCommandConfig> result;
            result = CacheManager.GetWithLocalCache<Dictionary<string, DataCommandConfig>>("DA_GetAllDataCommandConfigInfosFromCache", () =>
            {
                List<string> list;
                var dictionary = GetAllDataCommandConfigInfos(out list);
                CacheFactory.GetInstance().Add("DA_GetAllDataCommandConfigInfosFromCache", dictionary);
                return dictionary;
            });            
            return result;
        }
        private static Dictionary<string, DataCommandConfig> GetAllDataCommandConfigInfos(out List<string> configFileList)
        {
            DataCommandFileList dataCommandFileList = ConfigHelper.LoadSqlConfigListFile();
            Dictionary<string, DataCommandConfig> result;
            if (dataCommandFileList == null || dataCommandFileList.FileList == null || dataCommandFileList.FileList.Length <= 0)
            {
                configFileList = new List<string>(1);
                result = new Dictionary<string, DataCommandConfig>(0);
            }
            else
            {
                configFileList = new List<string>(dataCommandFileList.FileList.Length + 1);
                Dictionary<string, DataCommandConfig> dictionary = new Dictionary<string, DataCommandConfig>();
                DataCommandFileList.DataCommandFile[] fileList = dataCommandFileList.FileList;
                for (int i = 0; i < fileList.Length; i++)
                {
                    DataCommandFileList.DataCommandFile dataCommandFile = fileList[i];
                    string text = dataCommandFile.FileName;
                    string pathRoot = Path.GetPathRoot(text);
                    if (pathRoot == null || pathRoot.Trim().Length <= 0)
                    {
                        text = Path.Combine(ConfigHelper.ConfigFolder, text);
                    }
                    if (!string.IsNullOrWhiteSpace(text) && File.Exists(text))
                    {
                        configFileList.Add(text);
                    }
                    DataOperations dataOperations = ConfigHelper.LoadDataCommandList(text);
                    if (dataOperations != null && dataOperations.DataCommand != null && dataOperations.DataCommand.Length > 0)
                    {
                        DataCommandConfig[] dataCommand = dataOperations.DataCommand;
                        for (int j = 0; j < dataCommand.Length; j++)
                        {
                            DataCommandConfig dataCommandConfig = dataCommand[j];
                            if (dictionary.ContainsKey(dataCommandConfig.Name))
                            {
                                throw new FileLoadException(string.Concat(new string[]
                                {
                                    "Duplicate name '",
                                    dataCommandConfig.Name,
                                    "' for data command in file '",
                                    text,
                                    "'."
                                }));
                            }
                            dictionary.Add(dataCommandConfig.Name, dataCommandConfig);
                        }
                    }
                }
                result = dictionary;
            }
            return result;
        }
        public static DataCommandConfig GetDataCommandConfig(string sqlNameInConfig,out string connectionString)
        {
            Dictionary<string, DataCommandConfig> allDataCommandConfigInfosFromCache = GetAllDataCommandConfigInfosFromCache();
            if (!allDataCommandConfigInfosFromCache.ContainsKey(sqlNameInConfig))
            {
                throw new KeyNotFoundException("Can't find the data command configuration of name '" + sqlNameInConfig + "'");
            }
            var result= allDataCommandConfigInfosFromCache[sqlNameInConfig];
            ProviderType providerType;
            ConnectionStringManager.GetConnectionInfo(result.Database, out connectionString, out providerType);
            return result;
        }

        public static DataCommandConfig GetDataCommandConfig(string sqlNameInConfig)
        {
            Dictionary<string, DataCommandConfig> allDataCommandConfigInfosFromCache = GetAllDataCommandConfigInfosFromCache();
            if (!allDataCommandConfigInfosFromCache.ContainsKey(sqlNameInConfig))
            {
                throw new KeyNotFoundException("Can't find the data command configuration of name '" + sqlNameInConfig + "'");
            }
            var result = allDataCommandConfigInfosFromCache[sqlNameInConfig];
            return result;
        }

        public static string GetDataCommandSql(string sqlNameInConfig, out string connectionString)
        {
            Dictionary<string, DataCommandConfig> allDataCommandConfigInfosFromCache = GetAllDataCommandConfigInfosFromCache();
            if (!allDataCommandConfigInfosFromCache.ContainsKey(sqlNameInConfig))
            {
                throw new KeyNotFoundException("Can't find the data command configuration of name '" + sqlNameInConfig + "'");
            }
            var result = allDataCommandConfigInfosFromCache[sqlNameInConfig];
            ProviderType providerType;
            ConnectionStringManager.GetConnectionInfo(result.Database, out connectionString, out providerType);
            return result.CommandText;
        }

        public static string GetDataCommandSql(string sqlNameInConfig)
        {
            Dictionary<string, DataCommandConfig> allDataCommandConfigInfosFromCache = GetAllDataCommandConfigInfosFromCache();
            if (!allDataCommandConfigInfosFromCache.ContainsKey(sqlNameInConfig))
            {
                throw new KeyNotFoundException("Can't find the data command configuration of name '" + sqlNameInConfig + "'");
            }
            var result = allDataCommandConfigInfosFromCache[sqlNameInConfig];
            return result.CommandText;
        }
    }
}
#endif