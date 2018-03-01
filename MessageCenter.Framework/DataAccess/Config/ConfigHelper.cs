#if NETSTANDARD1_3 || NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MessageCenter.DataAccess
{
    internal static class ConfigHelper
    {
        private const string DEFAULT_SQL_CONFIG_LIST_FILE_PATH = "Configuration/Data/DbCommandFiles.config";
        private const string DEFAULT_DATABASE_LIST_FILE_PATH = "Configuration/Data/Database.config";
        private static string s_ConfigFolder = null;
        private static DataAccessSetting s_Setting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json",optional:true).Build().GetSection("DataAccessSetting").Get<DataAccessSetting>();

        public static string ConfigFolder
        {
            get
            {
                if (ConfigHelper.s_ConfigFolder == null)
                {
                    ConfigHelper.s_ConfigFolder = Path.GetDirectoryName(ConfigHelper.SqlConfigListFilePath);
                }
                return ConfigHelper.s_ConfigFolder;
            }
        }
        public static string SqlConfigListFilePath
        {
            get
            {
                string text = s_Setting.SqlConfigListFilePath??"Configuration/Data/DbCommandFiles.config";
                string pathRoot = Path.GetPathRoot(text);
                string result;
                if (pathRoot == null || pathRoot.Trim().Length <= 0)
                {
                    result = Path.Combine(Directory.GetCurrentDirectory(), text);
                }
                else
                {
                    result = text;
                }
                if(!string.IsNullOrWhiteSpace(s_Setting.EnvironmentVariable))
                {
                    result = result.Replace(".config", "{s_Setting.EnvironmentVariable}.config");
                }
                return result;
            }
        }
        public static string DatabaseListFilePath
        {
            get
            {
                string text = s_Setting.DatabaseListFilePath?? "Configuration/Data/Database.config";
                string pathRoot = Path.GetPathRoot(text);
                string result;
                if (pathRoot == null || pathRoot.Trim().Length <= 0)
                {
                    result = Path.Combine(Directory.GetCurrentDirectory(), text);
                }
                else
                {
                    result = text;
                }
                if (!string.IsNullOrWhiteSpace(s_Setting.EnvironmentVariable))
                {
                    result = result.Replace(".config", "{s_Setting.EnvironmentVariable}.config");
                }
                return result;
            }
        }
        private static T LoadFromXml<T>(string fileName)
        {
            FileStream fileStream = null;
            T result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                result = (T)((object)xmlSerializer.Deserialize(fileStream));
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
            return result;
        }
        public static DataCommandFileList LoadSqlConfigListFile()
        {
            string sqlConfigListFilePath = ConfigHelper.SqlConfigListFilePath;
            DataCommandFileList result;
            if (!string.IsNullOrWhiteSpace(sqlConfigListFilePath) && File.Exists(sqlConfigListFilePath.Trim()))
            {
                result = ConfigHelper.LoadFromXml<DataCommandFileList>(sqlConfigListFilePath.Trim());
            }
            else
            {
                result = null;
            }
            return result;
        }
        public static DatabaseList LoadDatabaseListFile()
        {
            return ConfigHelper.LoadFromXml<DatabaseList>(ConfigHelper.DatabaseListFilePath);
        }
        public static DataOperations LoadDataCommandList(string filePath)
        {
            return ConfigHelper.LoadFromXml<DataOperations>(filePath);
        }

        //public static DataCommandFileList LoadSqlConfigListFile()
        //{
        //    string sqlConfigListFilePath = ConfigHelper.SqlConfigListFilePath;
        //    DataCommandFileList result = null;
        //    if (!string.IsNullOrWhiteSpace(sqlConfigListFilePath) && File.Exists(sqlConfigListFilePath.Trim()))
        //    {
        //        result = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddXmlFile(sqlConfigListFilePath.Trim(), optional: true).Build().Get<DataCommandFileList>();
        //    }
        //    return result;
        //}
        //public static DatabaseList LoadDatabaseListFile()
        //{
        //    return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddXmlFile(ConfigHelper.DatabaseListFilePath, optional: true).Build().Get<DatabaseList>();
        //}
        //public static DataOperations LoadDataCommandList(string filePath)
        //{
        //    return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddXmlFile(filePath, optional: true).Build().Get<DataOperations>();
        //}
    }
}
#endif