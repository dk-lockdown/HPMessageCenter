using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageCenter.Framework.Utility
{
    public class RandomSeedUtility
    {
        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        } 
    }
}
