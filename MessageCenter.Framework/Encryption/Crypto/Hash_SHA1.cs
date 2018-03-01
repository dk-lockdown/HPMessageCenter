using System;
using System.Security.Cryptography;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public class Hash_SHA1 : ICrypto
    {
        private readonly SHA1 sha1;

        public Hash_SHA1()
        {
            sha1 = SHA1.Create();
        }

        public string Decrypt(string encryptedBase64String)
        {
            throw new ApplicationException("SHA1不可逆!");
        }

        public string Encrypt(string plainString)
        {
            byte[] hashValue = sha1.ComputeHash(Encoding.UTF8.GetBytes(plainString));
            return Convert.ToBase64String(hashValue);
        }

        public string Encrypt(string plainString, byte[] saltValue)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainString);

            byte[] SaltedPlainBytes = new byte[plainBytes.Length + saltValue.Length];

            plainBytes.CopyTo(SaltedPlainBytes, 0);
            saltValue.CopyTo(SaltedPlainBytes, plainBytes.Length);

            byte[] saltedencryptedBytes = sha1.ComputeHash(SaltedPlainBytes);

            byte[] encryptedBytes = new byte[saltedencryptedBytes.Length + saltValue.Length];
            saltedencryptedBytes.CopyTo(encryptedBytes, 0);
            saltValue.CopyTo(encryptedBytes, saltedencryptedBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Encrypt(string plainString, int saltLength)
        {
            byte[] saltValue = new byte[saltLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //用加密型强随机字节填充的数组
            rng.GetBytes(saltValue);
            return Encrypt(plainString, saltValue);
        }
    }
}
