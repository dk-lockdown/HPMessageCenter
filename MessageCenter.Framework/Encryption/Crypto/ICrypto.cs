using System;

namespace MessageCenter.Framework.Encryption
{
    public interface ICrypto
    {
        string Decrypt(string encryptedBase64ConnectString);
        string Encrypt(string plainConnectString);
    }
}

