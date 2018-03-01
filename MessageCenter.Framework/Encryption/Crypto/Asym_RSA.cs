using System;
using System.Security.Cryptography;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public class Asym_RSA : ICrypto
    {
        private string m_RsaKey = "<RSAKeyValue><Modulus>wOq0QHjSnNmS6qAySWCYhWhMfWZHyCz+u2kTdFSboVoRgAH4T+wobLydGXUVdi2XccJwjvZcPHOZ5vZpYY9Hf9fkpJfxpOwaIB2IV+owq0EFyCdhE7vTFHiZm2cfCo+T8m224KHrMEFsoAhd11eQzyhXIU2K7XHiX5Xu2Jtnn1s=</Modulus><Exponent>AQAB</Exponent><P>9g91q1gltBev0vWlfdkElVXcV7TIu99/nHo5DE5wDDQPGO2Fmtfy02rWlc1G9pm67xcdCgPQ8wKbJ1JuYPY99Q==</P><Q>yLWuJ2/R5Levg3h8f2RZ2EhnyN3+ht7t0sFtdKSBOroU8Mgtvsu6FGkYQdihqN3+mbe3nICq/GuROvg3MUVGDw==</Q><DP>XlYTAPwsiF1EdZbkOdmIHlDqx11yUEUhwbZCROuVnbgfyajWvkTovhGJ76jh+g16U8wCwCIya9il7291DguaOQ==</DP><DQ>AfEQAD2qsCW+wuzVd34HCHqa1myfW7qoXlOUtX4p6eGG9lVZa/EYmb3yiCCKX9HV9rK6Sf9MqCh6PTHNhuJ+rQ==</DQ><InverseQ>yye+av62y1KJrvhUGtw5wqY0rBW8aCwywlqAy4+gOU8OsoNpzSW5j1rLNz7vZxCv/smrVDPm2hvJN745Ln3yow==</InverseQ><D>s/xFn8EZ/myfvXcoc31Dz3O3qWc7oW8ZWhB2rhoh+S/nE97CpQ5XyNtQVuf91fxDR0d5bGg9NclE1U8gknzy3prh4WoRsDv9ik44Dge8FvlFotAWuRJeSlla55m3mv9EcoKq9mxxDAMTin1Bnd70yE/HAnzybgSgFQUhNNxh4kE=</D></RSAKeyValue>";

        public string Decrypt(string encryptedBase64ConnectString)
        {
            byte[] rgb = Convert.FromBase64String(encryptedBase64ConnectString);
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
            provider.FromXmlString(this.m_RsaKey);
            byte[] bytes = provider.Decrypt(rgb, false);
            return Encoding.Unicode.GetString(bytes);
        }

        public string Encrypt(string plainConnectString)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(plainConnectString);
            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
            provider.FromXmlString(this.m_RsaKey);
            byte[] inArray = provider.Encrypt(bytes, false);
            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }
    }
}

