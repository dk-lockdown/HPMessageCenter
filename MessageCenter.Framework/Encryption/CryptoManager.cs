using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public static class CryptoManager
    {
        public static ICrypto GetCrypto(CryptoAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case CryptoAlgorithm.DES:
                    return new Sym_DES();

                case CryptoAlgorithm.RC2:
                    return new Sym_RC2();

                case CryptoAlgorithm.Rijndael:
                    return new Sym_Rijndael();

                case CryptoAlgorithm.TripleDES:
                    return new Sym_TripleDES();

                case CryptoAlgorithm.RSA:
                    return new Asym_RSA();

                case CryptoAlgorithm.MD5:
                    return new Hash_MD5();

                case CryptoAlgorithm.SHA1:
                    return new Hash_SHA1();
            }
            return null;
        }
        private static byte[] desIV = new byte[] { 0x1d, 0x87, 0x34, 9, 0x41, 3, 0x61, 0x62 };
        private static byte[] desKey = new byte[] { 1, 0x4d, 0x54, 0x22, 0x45, 90, 0x17, 0x2c };

        public static string Decrypt(string encryptedText)
        {
            MemoryStream stream = new MemoryStream(200);
            stream.SetLength(0);
            byte[] buffer = Convert.FromBase64String(encryptedText);
            DES des = new DESCryptoServiceProvider
            {
                KeySize = 0x40
            };
            CryptoStream stream2 = new CryptoStream(stream, des.CreateDecryptor(desKey, desIV), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] buffer2 = new byte[stream.Length];
            stream.Read(buffer2, 0, buffer2.Length);
            stream2.Close();
            stream.Close();
            return Encoding.Unicode.GetString(buffer2);
        }

        public static string Encrypt(string plainText)
        {
            MemoryStream stream = new MemoryStream(200);
            stream.SetLength(0);
            byte[] bytes = Encoding.Unicode.GetBytes(plainText);
            DES des = new DESCryptoServiceProvider();
            CryptoStream stream2 = new CryptoStream(stream, des.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream2.Close();
            stream.Close();
            return Convert.ToBase64String(buffer, 0, buffer.Length);
        }
    }
}

