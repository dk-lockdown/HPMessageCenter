using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Framework.Extension
{
    public class PasswordFormater
    {
        public static string GenerateSalt()
        {
            byte[] data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public static string EncodePassword(string password, string salt, PasswordHashAlgorithm pha)
        {
            // 将密码和salt值转换成字节形式并连接起来
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] src = Convert.FromBase64String(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            byte[] inArray = null;
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            // 用SHA1算法，对连接后的值进行散列
            HashAlgorithm algorithm = null;
            switch (pha)
            {
                case PasswordHashAlgorithm.sha1:
                    algorithm = SHA1.Create();
                    break;
                case PasswordHashAlgorithm.md5:
                    algorithm = MD5.Create();
                    break;
            }
            if (algorithm == null)
            {
                throw new Exception("HashAlgorithm Is Null!");
            }
            inArray = algorithm.ComputeHash(dst);
            // 以字符串形式返回散列值
            return Convert.ToBase64String(inArray);
        }
    }

    public enum PasswordHashAlgorithm
    {
        sha1 = 0,
        md5 = 1
    }
}
