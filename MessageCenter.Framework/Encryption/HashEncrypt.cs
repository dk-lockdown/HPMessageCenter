using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public class HashEncrypt
    {
        public static string SHA1Encrypt(string data)
        {
            System.Security.Cryptography.SHA1 sHA = new System.Security.Cryptography.SHA1Managed();
            byte[] value = sHA.ComputeHash(System.Text.Encoding.ASCII.GetBytes(data));
            string text = System.BitConverter.ToString(value);
            text = text.Replace("-", "");
            sHA.Clear();
            return text;
        }
        public static string SHA256Encrypt(string data)
        {
            System.Security.Cryptography.SHA256 sHA = new System.Security.Cryptography.SHA256Managed();
            byte[] value = sHA.ComputeHash(System.Text.Encoding.ASCII.GetBytes(data));
            string text = System.BitConverter.ToString(value);
            text = text.Replace("-", "");
            sHA.Clear();
            return text;
        }
        public static string SHA512Encrypt(string data)
        {
            System.Security.Cryptography.SHA512 sHA = new System.Security.Cryptography.SHA512Managed();
            byte[] value = sHA.ComputeHash(System.Text.Encoding.ASCII.GetBytes(data));
            string text = System.BitConverter.ToString(value);
            text = text.Replace("-", "");
            sHA.Clear();
            return text;
        }
        public static string DESEncrypt(string originalValue, string key, string IV)
        {
            IV += "12345678";
            key = key.Substring(0, 8);
            IV = IV.Substring(0, 8);
            System.Security.Cryptography.ICryptoTransform transform = new System.Security.Cryptography.DESCryptoServiceProvider
            {
                Key = System.Text.Encoding.UTF8.GetBytes(key),
                IV = System.Text.Encoding.UTF8.GetBytes(IV)
            }.CreateEncryptor();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(originalValue);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, transform, System.Security.Cryptography.CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            return System.Convert.ToBase64String(memoryStream.ToArray());
        }
        public static string DESEncrypt(string originalValue, string key)
        {
            return HashEncrypt.DESEncrypt(originalValue, key, key);
        }
        public static string DESDecrypt(string encryptedValue, string key, string IV)
        {
            key += "12345678";
            IV += "12345678";
            key = key.Substring(0, 8);
            IV = IV.Substring(0, 8);
            System.Security.Cryptography.ICryptoTransform transform = new System.Security.Cryptography.DESCryptoServiceProvider
            {
                Key = System.Text.Encoding.UTF8.GetBytes(key),
                IV = System.Text.Encoding.UTF8.GetBytes(IV)
            }.CreateDecryptor();
            byte[] array = System.Convert.FromBase64String(encryptedValue);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, transform, System.Security.Cryptography.CryptoStreamMode.Write);
            cryptoStream.Write(array, 0, array.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        public static string DESDecrypt(string encryptedValue, string key)
        {
            return HashEncrypt.DESDecrypt(encryptedValue, key, key);
        }
    }
}
