using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public class Sym_TripleDES : ICrypto
    {
        private static byte[] s_DesIV = new byte[] { 0x1d, 0x87, 0x34, 9, 0x41, 3, 0x61, 0x62 };
        private static byte[] s_DesKey = new byte[] { 
            0xb9, 0xe9, 0x72, 0xf8, 40, 0x55, 0xd7, 0x40, 0xa1, 0xfc, 0x5e, 0x8e, 0x5d, 0x15, 0xa4, 0xe8, 
            0xa7, 0x84, 0xbc, 0xe3, 0x9a, 0x69, 0xde, 0x63
         };

        public string Decrypt(string encryptedBase64ConnectString)
        {
            MemoryStream stream = new MemoryStream(200);
            stream.SetLength(0L);
            byte[] buffer = Convert.FromBase64String(encryptedBase64ConnectString);
            TripleDES edes = new TripleDESCryptoServiceProvider();
            edes.KeySize = 0xc0;
            CryptoStream stream2 = new CryptoStream(stream, edes.CreateDecryptor(s_DesKey, s_DesIV), CryptoStreamMode.Write);
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
            TripleDES edes = new TripleDESCryptoServiceProvider();
            CryptoStream stream2 = new CryptoStream(stream, edes.CreateEncryptor(s_DesKey, s_DesIV), CryptoStreamMode.Write);
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

