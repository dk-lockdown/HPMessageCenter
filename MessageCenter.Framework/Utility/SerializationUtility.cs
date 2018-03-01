using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MessageCenter.Framework.Utility
{
    public static class SerializationUtility
    {
        public static T LoadFromXml<T>(string filePath)
        {
            FileStream fileStream = null;
            T result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                result = (T)((object)xmlSerializer.Deserialize(fileStream));
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            return result;
        }
        public static void SaveToXml(string filePath, object data)
        {
            FileStream fileStream = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
                fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                xmlSerializer.Serialize(fileStream, data);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        #region XML序列化
        public static string ToXmlString(this object serialObject, bool removeDataRootXmlNode = false)
        {
            return XmlSerialize(serialObject, removeDataRootXmlNode);
        }
        public static string XmlSerialize(object serialObject, bool removeDataRootXmlNode = false)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer xmlSerializer = new XmlSerializer(serialObject.GetType());
            string result;
            using (TextWriter textWriter = new StringWriter(sb))
            {
                xmlSerializer.Serialize(textWriter, serialObject);
                string text = textWriter.ToString();
                if (removeDataRootXmlNode)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(text);
                    text = xmlDocument.LastChild.InnerXml;
                }
                result = text;
            }
            return result;
        }
        public static object XmlDeserialize(string str, System.Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            object result;
            using (TextReader textReader = new StringReader(str))
            {
                result = xmlSerializer.Deserialize(textReader);
            }
            return result;
        }
        public static T XmlDeserialize<T>(string str)
        {
            return (T)((object)XmlDeserialize(str, typeof(T)));
        }

        #endregion

        #region 二进制序列化
        public static string ToBinaryBase64String(this object serialObject)
        {
            return BinarySerialize(serialObject);
        }
        public static string BinarySerialize(object serialObject)
        {
            string result;
            if (serialObject == null)
            {
                result = string.Empty;
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, serialObject);
                    memoryStream.Position = 0L;
                    result = System.Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return result;
        }
        public static object BinaryDeserialize(string str)
        {
            object result;
            if (str == null || str.Trim().Length <= 0)
            {
                result = null;
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream(System.Convert.FromBase64String(str)))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    result = binaryFormatter.Deserialize(memoryStream);
                }
            }
            return result;
        }
        public static T BinaryDeserialize<T>(string str)
        {
            return (T)((object)BinaryDeserialize(str));
        }

        #endregion

        #region Json序列化
        public static string ToJsonString(this object serialObject)
        {
            return JsonSerialize(serialObject);
        }
        public static string JsonSerialize(object serialObject)
        {
            string result;
            if (serialObject == null)
            {
                result = string.Empty;
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(serialObject.GetType());
                    dataContractJsonSerializer.WriteObject(memoryStream, serialObject);
                    byte[] array = memoryStream.ToArray();
                    result = Encoding.UTF8.GetString(array, 0, array.Length);
                }
            }
            return result;
        }
        public static object JsonDeserialize(string str, System.Type type)
        {
            object result;
            if (str == null || str.Trim().Length <= 0)
            {
                result = null;
            }
            else
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                {
                    DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(type);
                    result = dataContractJsonSerializer.ReadObject(stream);
                }
            }
            return result;
        }
        public static T JsonDeserialize<T>(string str)
        {
            return (T)((object)JsonDeserialize(str, typeof(T)));
        }

        #endregion

        public static T DeepClone<T>(T t)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            T result;
            using (Stream stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, t);
                stream.Position = 0L;
                result = (T)((object)binaryFormatter.Deserialize(stream));
            }
            return result;
        }
    }
}