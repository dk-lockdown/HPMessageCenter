using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public class Sym_DES : ICrypto
    {
        private static byte[] s_DesIV = new byte[] { 0x1d, 0x87, 0x34, 9, 0x41, 3, 0x61, 0x62 };
        private static byte[] s_DesKey = new byte[] { 1, 0x4d, 0x54, 0x22, 0x45, 90, 0x17, 0x2c };

        public string Decrypt(string encryptedBase64ConnectString)
        {
            MemoryStream stream = new MemoryStream(200);
            stream.SetLength(0L);
            byte[] buffer = Convert.FromBase64String(encryptedBase64ConnectString);
            DES des = new DESCryptoServiceProvider();
            des.KeySize = 0x40;
            CryptoStream stream2 = new CryptoStream(stream, des.CreateDecryptor(s_DesKey, s_DesIV), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            stream.Flush();
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] buffer2 = new byte[stream.Length];
            stream.Read(buffer2, 0, buffer2.Length);
            stream2.Close();
            stream.Close();
            return Encoding.Unicode.GetString(buffer2);
        }

        public string Encrypt(string plainConnectString)
        {
            MemoryStream stream = new MemoryStream(200);
            stream.SetLength(0L);
            byte[] bytes = Encoding.Unicode.GetBytes(plainConnectString);
            DES des = new DESCryptoServiceProvider();
            CryptoStream stream2 = new CryptoStream(stream, des.CreateEncryptor(s_DesKey, s_DesIV), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream.Flush();
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream2.Close();
            stream.Close();
            return Convert.ToBase64String(buffer, 0, buffer.Length);
        }
    }
}

